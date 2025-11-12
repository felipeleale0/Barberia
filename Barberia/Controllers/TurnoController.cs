using Barberia.Data;
using Barberia.Models;
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

        // 📋 Listado de turnos
        public IActionResult Index()
        {
            var turnos = _context.Turnos
                .Include(t => t.Barbero)
                .Include(t => t.Servicio)
                .ToList();

            return View(turnos);
        }

        // 🆕 Formulario nuevo turno
        [HttpGet]
        public IActionResult Crear()
        {
            ViewBag.Barberos = _context.Barberos.ToList();
            ViewBag.Servicios = _context.Servicios.ToList();

            return View();
        }

        // 💾 Guardar turno
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Turno turno)
        {
            if (ModelState.IsValid)
            {
                // Buscar el servicio y asignar el precio
                var servicio = await _context.Servicios.FindAsync(turno.ServicioId);
                if (servicio != null)
                {
                    turno.Precio = servicio.Precio;
                }

                _context.Turnos.Add(turno);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = "✔ Turno registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            // Si el modelo no es válido, recargar listas
            ViewBag.Barberos = _context.Barberos.ToList();
            ViewBag.Servicios = _context.Servicios.ToList();
            TempData["Error"] = "❌ Ocurrió un error al guardar el turno.";
            return View(turno);
        }

        [HttpGet]
        public IActionResult ObtenerServiciosPorBarbero(int barberoId)
        {
            var servicios = _context.BarberoServicios
                .Where(bs => bs.BarberoId == barberoId)
                .Select(bs => new { id = bs.Servicio.Id, nombre = bs.Servicio.Nombre })
                .ToList();

            return Json(servicios);
        }
    }
}