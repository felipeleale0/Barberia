using System.Security.Claims;
using Barberia.Data;
using Barberia.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Barberia.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly BarberiaContext _context;

        public ReservasController(BarberiaContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idStr, out var id) ? id : 0;
        }

        // ============= ADMIN: VER TODAS LAS RESERVAS =============

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Index()
        {
            var reservas = await _context.Reservas
                .Include(r => r.Turno)
                    .ThenInclude(t => t.Empleado)
                        .ThenInclude(e => e.Persona)
                .Include(r => r.Usuario)
                    .ThenInclude(u => u.Persona)
                .Include(r => r.Servicio)
                .Include(r => r.Estado)
                .OrderBy(r => r.Turno.Fecha)
                .ThenBy(r => r.Turno.Hora)
                .ToListAsync();

            return View(reservas);
        }

        // ============= USUARIO: MIS RESERVAS =============

        public async Task<IActionResult> Mis()
        {
            var userId = GetCurrentUserId();

            var reservas = await _context.Reservas
                .Include(r => r.Turno)
                    .ThenInclude(t => t.Empleado)
                        .ThenInclude(e => e.Persona)
                .Include(r => r.Servicio)
                .Include(r => r.Estado)
                .Where(r => r.UsuarioId == userId)
                .OrderBy(r => r.Turno.Fecha)
                .ThenBy(r => r.Turno.Hora)
                .ToListAsync();

            return View(reservas);
        }

        // ============= CREAR RESERVA (desde Turnos/Disponibles) =============

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int turnoId, int servicioId)
        {
            var userId = GetCurrentUserId();
            if (userId == 0)
                return Unauthorized();

            var turno = await _context.Turnos
                .FirstOrDefaultAsync(t => t.Id == turnoId);

            if (turno == null)
            {
                TempData["Error"] = "El turno no existe.";
                return RedirectToAction("Disponibles", "Turnos");
            }

            if (!turno.EstaDisponible)
            {
                TempData["Error"] = "El turno ya fue reservado por otra persona.";
                return RedirectToAction("Disponibles", "Turnos");
            }

            var servicio = await _context.Servicios
                .FirstOrDefaultAsync(s => s.Id == servicioId);

            if (servicio == null)
            {
                TempData["Error"] = "Servicio inválido.";
                return RedirectToAction("Disponibles", "Turnos");
            }

            var estadoPendiente = await _context.Estados
                .FirstOrDefaultAsync(e => e.Descripcion == "Pendiente");

            if (estadoPendiente == null)
            {
                TempData["Error"] = "No está configurado el estado 'Pendiente'.";
                return RedirectToAction("Disponibles", "Turnos");
            }

            var reserva = new Reserva
            {
                TurnoId = turno.Id,
                UsuarioId = userId,
                ServicioId = servicio.Id,
                EstadoId = estadoPendiente.Id,
                CostoTotal = servicio.Precio  // asumiendo que Servicio tiene Precio
            };

            // Marcar el turno como ocupado
            turno.EstaDisponible = false;

            try
            {
                _context.Reservas.Add(reserva);
                await _context.SaveChangesAsync();

                TempData["Ok"] = "Turno reservado correctamente.";
                return RedirectToAction("Mis");
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al confirmar el turno.";
                // por seguridad dejamos el turno disponible
                turno.EstaDisponible = true;
                await _context.SaveChangesAsync();
                return RedirectToAction("Disponibles", "Turnos");
            }
        }

        // ============= CANCELAR RESERVA (desde Mis turnos) =============

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var userId = GetCurrentUserId();

            var reserva = await _context.Reservas
                .Include(r => r.Turno)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
                return NotFound();

            if (reserva.UsuarioId != userId)
                return Forbid();

            var estadoCancelado = await _context.Estados
                .FirstOrDefaultAsync(e => e.Descripcion == "Cancelado");

            if (estadoCancelado != null)
            {
                reserva.EstadoId = estadoCancelado.Id;
            }

            if (reserva.Turno != null)
            {
                reserva.Turno.EstaDisponible = true;
            }

            await _context.SaveChangesAsync();

            TempData["Ok"] = "Reserva cancelada.";
            return RedirectToAction("Mis");
        }
    }
}
