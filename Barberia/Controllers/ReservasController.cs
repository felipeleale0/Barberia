using Barberia.Data;
using Barberia.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Barberia.Controllers
{
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


        // GET: Reservas
        [HttpGet]
        public async Task<IActionResult> Reservas()
        {
            var currentUserId = GetCurrentUserId();

            Console.WriteLine("validamos que el Id es: " + currentUserId);

            var user = await _context.Usuarios.FindAsync(currentUserId);

            Console.WriteLine("validamos que el Usuario es: " + user?.EsAdmin);


            var listaDeUsuarios = _context.Reservas
                .Include(r => r.Estado)
                .Include(r => r.Servicio)
                .Include(r => r.Turno)
                .Include(r => r.Usuario)
                .AsQueryable();


            if (user?.EsAdmin == false)
            {
                listaDeUsuarios = listaDeUsuarios
                    .Where(r => r.UsuarioId == currentUserId);
            }

            return View(await listaDeUsuarios.ToListAsync());
        }

        // POST: Reserva/Cancelar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Turno)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null)
                return NotFound();

            if (reserva.EstadoId == 3)
            {
                TempData["Error"] = "No es posible cancelar un turno previamente cancelado.";
                return RedirectToAction(nameof(Reservas));

            }

            reserva.EstadoId = 3;

            reserva.Turno.EstaDisponible = true;

            await _context.SaveChangesAsync();

            TempData["OK"] = "reserva cancelada correctamente.";
            return RedirectToAction(nameof(Reservas));
        }



        // POST: Reserva/Re-agendar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reagendar(int id)
        {
            // id = Id de la reserva que quiero mover a otro turno

            return RedirectToAction(
                actionName: "Index",
                controllerName: "Turnos",
                routeValues: new { reservaId = id } // pasamos el id
            );

        }


    }
}
