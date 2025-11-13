//using Barberia.Data;
//using Barberia.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;

//namespace Barberia.Controllers
//{
//    public class BarberosController : Controller
//    {
//        private readonly BarberiaContext _context;

//        public BarberosController(BarberiaContext context)
//        {
//            _context = context;
//        }

//        // LISTADO DE BARBEROS
//        public IActionResult Index()
//        {
//            var barberos = _context.Barberos.Include(b => b.BarberoServicios).ThenInclude(bs => bs.Servicio).ToList();
//            return View(barberos);
//        }

//        // FORMULARIO NUEVO BARBERO
//        [HttpGet]
//        public IActionResult Crear()
//        {
//            ViewBag.Servicios = _context.Servicios.ToList();
//            return View();
//        }

//        // GUARDAR NUEVO BARBERO
//        [HttpPost]
//        public IActionResult Crear(Barbero barbero, int[] serviciosSeleccionados)
//        {
//            if (ModelState.IsValid)
//            {
//                _context.Barberos.Add(barbero);
//                _context.SaveChanges();

//                foreach (var id in serviciosSeleccionados)
//                {
//                    var relacion = new BarberoServicio
//                    {
//                        BarberoId = barbero.Id,
//                        ServicioId = id
//                    };
//                    _context.BarberoServicios.Add(relacion);
//                }

//                _context.SaveChanges();
//                return RedirectToAction(nameof(Index));
//            }

//            ViewBag.Servicios = _context.Servicios.ToList();
//            return View(barbero);
//        }
//    }
//}