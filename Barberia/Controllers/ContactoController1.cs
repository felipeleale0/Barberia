using Microsoft.AspNetCore.Mvc;

namespace Barberia.Controllers
{
    public class ContactoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
