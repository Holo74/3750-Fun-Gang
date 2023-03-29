using Microsoft.AspNetCore.Mvc;
using Assignment_1.Data;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace Assignment_1.Controllers
{
    public class StudentRegistrationController : Controller
    {
        private readonly Assignment_1Context _context;
        private IMemoryCache _cache;

        public StudentRegistrationController(Assignment_1Context context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IActionResult Index()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");

            if (UserID != null)
            {
                Models.FilteredRegistrationAndClass FRC = new Models.FilteredRegistrationAndClass();
                FRC.VRC = CourseList()?.ToList();
                FRC.ClassDepartments = (from c in _context.Class select c.Department).Distinct().ToList();
                return View(FRC);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult FilteredCourseList()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");

            if (UserID != null)
            {
                Models.FilteredRegistrationAndClass FRC = new Models.FilteredRegistrationAndClass();
                FRC.VRC = CourseList(Request.Form["department"], Request.Form["keyword"])?.ToList();
                FRC.ClassDepartments = (from c in _context.Class select c.Department).Distinct().ToList();
                return View("Index", FRC);
            }
            return NotFound();
        }

        private IEnumerable<Models.ViewRegistrationAndClass>? CourseList(string department = "none", string keyword = "")
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
            );
            if (!department.Equals("none"))
            {
                CourseQuery = CourseQuery.Where(c => c.Department.Equals(department));
            }

            if (keyword.Length > 0)
            {
                CourseQuery = CourseQuery.Where(c => c.CourseName.Contains(keyword));
            }

            var CourseList = CourseQuery.ToList();
            var Course =
            (
                from c in CourseList
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
            await RegisterForClassLogic(UserID.Value, classId);
            _cache.Remove(CacheKeys.UserView);
            return Redirect("Index");
        }
        public async Task RegisterForClassLogic(int UserID, int classId)
        {
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
        }
    }
}