using Microsoft.AspNetCore.Mvc;

namespace Assignment_1.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
