using Microsoft.AspNetCore.Mvc;

namespace MountainGoats.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
