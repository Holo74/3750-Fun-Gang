using Microsoft.AspNetCore.Mvc;

namespace Assignment_1.Controllers
{
    public class SubmissionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
