using Barberia.Data;
using Barberia.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Barberia.Controllers
{   
    [Authorize(Policy = "AdminOnly")]
    public class ServicioController : Controller
    {
        private readonly BarberiaContext _context;

        public ServicioController(BarberiaContext context)
        {
            _context = context;
        }

        // GET: Servicio
        public async Task<IActionResult> Index()
        {
            var servicios = await _context.Servicios
                .OrderBy(s => s.Nombre)
                .ToListAsync();

            return View(servicios);
        }

        // POST: Servicio/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(string nombre, decimal precio, string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(nombre) || precio <= 0)
            {
                TempData["Error"] = "Debe ingresar un nombre y un precio válidos.";
                return RedirectToAction(nameof(Index));
            }

            var servicio = new Servicio
            {
                Nombre = nombre.Trim(),
                Precio = precio,
                Descripcion = descripcion
            };

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();

            TempData["OK"] = "Servicio creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Servicio/Editar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, string nombre, decimal precio, string? descripcion)
        {
            var servicio = await _context.Servicios.FindAsync(id);

            if (servicio == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(nombre) || precio <= 0)
            {
                TempData["Error"] = "Datos inválidos.";
                return RedirectToAction(nameof(Index));
            }

            servicio.Nombre = nombre.Trim();
            servicio.Precio = precio;
            servicio.Descripcion = descripcion;

            await _context.SaveChangesAsync();

            TempData["OK"] = "Servicio actualizado.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Servicio/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);

            if (servicio == null)
                return NotFound();

            // Si tiene reservas NO lo dejamos borrar
            var tieneReservas = await _context.Reservas.AnyAsync(r => r.ServicioId == id);
            if (tieneReservas)
            {
                TempData["Error"] = "No se puede eliminar un servicio que ya fue reservado.";
                return RedirectToAction(nameof(Index));
            }

            _context.Servicios.Remove(servicio);
            await _context.SaveChangesAsync();

            TempData["OK"] = "Servicio eliminado.";
            return RedirectToAction(nameof(Index));
        }
    }
}
