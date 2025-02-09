using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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
