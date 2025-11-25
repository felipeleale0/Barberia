using Barberia.Data;
using Barberia.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Barberia.Controllers
{
    public class TurnosController : Controller
    {
        private readonly BarberiaContext _context;

        public TurnosController(BarberiaContext context)
        {
            _context = context;
        }

        // ===================== ADMIN: ABM DE TURNOS =====================

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Index()
        {
            var turnos = await _context.Turnos
                .Include(t => t.Empleado)
                    .ThenInclude(e => e.Persona)
                .OrderBy(t => t.Fecha)
                .ThenBy(t => t.Hora)
                .ToListAsync();

            return View(turnos);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Empleados = _context.Empleados
                .Include(e => e.Persona)
                .ToList();

            return View();
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Turno turno)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Empleados = _context.Empleados
                    .Include(e => e.Persona)
                    .ToList();
                return View(turno);
            }

            turno.EstaDisponible = true;

            _context.Turnos.Add(turno);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===================== USUARIO: VER TURNOS DISPONIBLES =====================

        [Authorize] // cualquier usuario logueado
        public async Task<IActionResult> Disponibles(DateTime? fecha)
        {
            var query = _context.Turnos
                .Include(t => t.Empleado)
                    .ThenInclude(e => e.Persona)
                .Where(t => t.EstaDisponible);

            if (fecha.HasValue)
            {
                var f = fecha.Value.Date;
                query = query.Where(t => t.Fecha.Date == f);
            }

            var turnos = await query
                .OrderBy(t => t.Fecha)
                .ThenBy(t => t.Hora)
                .ToListAsync();

            ViewBag.FechaSeleccionada = fecha?.ToString("yyyy-MM-dd");
            ViewBag.Servicios = await _context.Servicios.ToListAsync();

            return View(turnos);
        }
    }
}
