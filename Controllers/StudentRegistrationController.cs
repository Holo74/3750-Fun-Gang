using Microsoft.AspNetCore.Mvc;
using Assignment_1.Data;
using System.Linq;

namespace Assignment_1.Controllers
{
    public class StudentRegistrationController : Controller
    {
        private readonly Assignment_1Context _context;

        public StudentRegistrationController(Assignment_1Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");

            var Registered =
            (
                from r in _context.Registrations select r
            );
            Registered = Registered.Where(reg => reg.UserFK == UserID);

            var listedReg = Registered.ToList();

            var Course =
            (
                from c in _context.Class
                join r in listedReg on c.ClassId equals r.ClassFK
                select new Models.ViewRegistrationAndClass()
            );
            if (UserID != null)
            {

                return View(Course);
            }
            return NotFound();
        }

        [HttpPost]
        public void RegisterForClass(int classId)
        {

        }
    }
}