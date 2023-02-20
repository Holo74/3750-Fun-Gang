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

            if (UserID != null)
            {

                return View(CourseList());
            }
            return NotFound();
        }

        private IEnumerable<Models.ViewRegistrationAndClass>? CourseList()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");

            var Registered =
            (
                from r in _context.Registrations select r
            );
            Registered = Registered.Where(reg => reg.UserFK == UserID);

            var listedReg = Registered.ToList();

            var CourseQuery =
            (
                from c in _context.Class select c
            ).ToList();

            var Course =
            (
                from c in CourseQuery
                join r in listedReg on c.ClassId equals r.ClassFK
                into Reg
                from r in Reg.DefaultIfEmpty()
                select new Models.ViewRegistrationAndClass { Classes = c, Reg = r }
            );
            return Course;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterForClass(int classId)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var Registered = from r in _context.Registrations select r;
            Registered = Registered.Where(reg => reg.UserFK == UserID && reg.ClassFK == classId);

            var listing = Registered.ToList();
            Models.Registrations reg = new Models.Registrations()
            {
                UserFK = UserID,
                ClassFK = classId,
                IsRegistered = 1
            };

            if (listing.Count == 0)
            {
                _context.Registrations.Add(reg);
            }
            else
            {
                var model = _context.Registrations.FirstOrDefault(id => id.ID == listing[0].ID);
                reg.ID = listing[0].ID;
                if (listing[0].IsRegistered == 1)
                {
                    reg.IsRegistered = 0;
                }
                model.IsRegistered = reg.IsRegistered;

            }
            _context.SaveChanges();
            return Redirect("Index");
        }
    }
}