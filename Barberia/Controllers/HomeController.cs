using Microsoft.AspNetCore.Mvc;
using Barberia.Models;

using Microsoft.AspNetCore.Mvc;

namespace Barberia.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
