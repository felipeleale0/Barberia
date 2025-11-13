using Microsoft.AspNetCore.Mvc;
using Barberia.Data;
using Barberia.Models.Domain;

namespace Barberia.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly BarberiaContext _context;

        public ServiciosController(BarberiaContext context)
        {
            _context = context;
        }

        // Mostrar lista de servicios
        public IActionResult Index()
        {
            var servicios = _context.Servicios.ToList();
            return View(servicios);
        }

        // Crear servicio
        [HttpGet]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                _context.Servicios.Add(servicio);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "✔ Servicio agregado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ Ocurrió un error al agregar el servicio.";
            return View(servicio);
        }

        // Editar servicio
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var servicio = _context.Servicios.Find(id);
            if (servicio == null)
                return NotFound();

            return View(servicio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Servicio servicio)
        {
            if (ModelState.IsValid)
            {
                _context.Servicios.Update(servicio);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "✔ Servicio actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "❌ Error al actualizar el servicio.";
            return View(servicio);
        }

        // Eliminar servicio
        [HttpGet]
        public IActionResult Eliminar(int id)
        {
            var servicio = _context.Servicios.Find(id);
            if (servicio == null)
                return NotFound();

            return View(servicio);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var servicio = await _context.Servicios.FindAsync(id);
            if (servicio != null)
            {
                _context.Servicios.Remove(servicio);
                await _context.SaveChangesAsync();
                TempData["Mensaje"] = "🗑 Servicio eliminado correctamente.";
            }
            else
            {
                TempData["Error"] = "❌ No se encontró el servicio.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
