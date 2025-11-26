// Controllers/TurnoController.cs
using Barberia.Data;
using Barberia.Models.Domain;
using Barberia.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Barberia.Controllers
{
    [Authorize] // sólo usuarios logueados
    public class TurnosController : Controller
    {
        private readonly BarberiaContext _context;

        public TurnosController(BarberiaContext context)
        {
            _context = context;
        }

        // GET: /Turno?fecha=2025-11-20
        public async Task<IActionResult> Index(DateTime? fecha)
        {
            var fechaSeleccionada = fecha?.Date ?? DateTime.Today;

            var turnos = await _context.Turnos
                .Include(t => t.Empleado)
                    .ThenInclude(e => e.Persona)
                .Include(t => t.Empleado.BarberoServicios)
                    .ThenInclude(bs => bs.Servicio)
                .Where(t => t.Fecha.Date == fechaSeleccionada)
                .OrderBy(t => t.Hora)
                .ToListAsync();

            var vm = new TurnoDisponibleViewModel
            {
                FechaSeleccionada = fechaSeleccionada,
                Turnos = turnos.Select(t => new TurnoDisponibleViewModel.ItemTurno
                {
                    TurnoId = t.Id,
                    Hora = t.Hora.ToString(@"hh\:mm"),
                    EmpleadoNombre = $"{t.Empleado.Persona.Nombre} {t.Empleado.Persona.Apellido}",
                    EstaDisponible = t.EstaDisponible,
                    Servicios = t.Empleado.BarberoServicios
                        .Select(bs => new TurnoDisponibleViewModel.ServicioItem
                        {
                            Id = bs.Servicio.Id,
                            Nombre = bs.Servicio.Nombre,
                            Precio = bs.Servicio.Precio
                        }).ToList()
                }).ToList()
            };

            return View(vm);
        }

        // POST: /Turno/Reservar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reservar(int turnoId, int servicioId, int? reservaAnteriorId)
        {
            var turno = await _context.Turnos
                .Include(t => t.Reservas)
                .FirstOrDefaultAsync(t => t.Id == turnoId);

            if (turno == null || !turno.EstaDisponible)
            {
                TempData["Error"] = "El turno ya no está disponible.";
                return RedirectToAction(nameof(Index), new { fecha = DateTime.Today });
            }

            var servicio = await _context.Servicios.FindAsync(servicioId);

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == User.Identity!.Name);

            var estadoReservado = await _context.Estados
                .FirstAsync(e => e.Descripcion == "Reservado");

            // 1. Crear la nueva reserva
            var reservaNueva = new Reserva
            {
                TurnoId = turno.Id,
                UsuarioId = usuario.Id,
                EstadoId = estadoReservado.Id,
                ServicioId = servicio.Id,
                CostoTotal = servicio.Precio
            };

            turno.EstaDisponible = false;
            _context.Reservas.Add(reservaNueva);

            // 2. Cancelar la reserva anterior (si aplica)
            if (reservaAnteriorId.HasValue)
            {
                var reservaVieja = await _context.Reservas
                    .Include(r => r.Turno)
                    .FirstOrDefaultAsync(r => r.Id == reservaAnteriorId.Value);

                if (reservaVieja != null)
                {
                    reservaVieja.EstadoId = 3; // Cancelada
                    reservaVieja.Turno.EstaDisponible = true;
                }
            }

            await _context.SaveChangesAsync();

            TempData["Ok"] = "Turno reagendado correctamente.";
            return RedirectToAction(nameof(Index), new { fecha = turno.Fecha });
        }

    }
}
