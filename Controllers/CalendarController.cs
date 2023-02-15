using Assignment_1.Data;
using Assignment_1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_1.Controllers
{
    public class CalendarController : Controller
    {
        private readonly Assignment_1Context _context; // declaration for the context object

        public CalendarController(Assignment_1Context context)
        {
            _context = context; // makes it so we can get the database at any time
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CalendarPage()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var classes = from c in _context.Class select c;
            classes = classes.Where(c => c.UserId == UserID);
            return View(classes);
        }
    }
}
