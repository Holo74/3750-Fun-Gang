using Assignment_1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_1.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CalendarPage()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            return Redirect("Index/?UserID=" + UserID);
        }
    }
}
