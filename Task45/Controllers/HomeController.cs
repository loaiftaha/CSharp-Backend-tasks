using Microsoft.AspNetCore.Mvc;

namespace Task45.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
