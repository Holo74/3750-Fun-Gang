using Assignment_1.Data;
using Assignment_1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Assignment_1.Controllers
{
    public class CourseController : Controller
    {
        private readonly Assignment_1Context _context; // declaration for the context object

        public CourseController(Assignment_1Context context)
        {
            _context = context; // makes it so we can get the database at any time
        }

        public IActionResult Index(int? classId)
        {
            //  classId = HttpContext.Session.GetInt32("ClassId");
            var UserID = HttpContext.Session.GetInt32("UserID");
            var courses = from c in _context.Class select c;
            var course = courses.Where(c => c.ClassId == classId).ToList();
            

            ClassUserAssignments CUA = new ClassUserAssignments();

            if (course.Count != 1)
            {
                return NotFound();
            }

            CUA.Class = course[0];

            // CourseInfo specificCourse = new CourseInfo();
            // specificCourse.CourseName = course[0].CourseName;
            // specificCourse.CourseNumber = course[0].CourseNumber;
            // specificCourse.Department = course[0].Department;

            // Does User teach the class?
            if (CUA.Class.UserId == UserID)
            {
                CUA.User = Models.Helper.ReturnFirstSelected<User>((from u in _context.User where u.Id == UserID select u).ToList());
                CUA.TeachesClass = true;
            }
            else
            {
                CUA.TeachesClass = false;
            }

            return View(CUA);
        }
        public IActionResult Details(int classId)
        {
            var courses = from c in _context.Class select c;
            var course = courses.Where(c => c.ClassId == classId).ToList();
            return View(); // just brings the create page up
        }


        public class CourseInfo
        {
            public string CourseName { get; set; }
            public string Department { get; set; }
            public int CourseNumber { get; set; }
            public bool TeachesClass { get; set; }
        }
    }
}
