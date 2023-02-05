using Microsoft.AspNetCore.Mvc;

namespace Assignment_1.Controllers
{
    public class ClassesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
