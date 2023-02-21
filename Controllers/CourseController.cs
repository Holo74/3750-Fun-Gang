using Assignment_1.Data;
using Assignment_1.Models;
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
            var UserID = HttpContext.Session.GetInt32("UserID");
            var courses = from c in _context.Class select c;
            var course = courses.Where(c => c.ClassId == classId).ToList();

            CourseInfo specificCourse = new CourseInfo();
            specificCourse.CourseName = course[0].CourseName;
            specificCourse.CourseNumber = course[0].CourseNumber;
            specificCourse.Department = course[0].Department;

            // Does User teach the class?
            if (course[0].UserId == UserID)
            {
                specificCourse.TeachesClass = true;
            }
            else { specificCourse.TeachesClass = false; }

            return View(specificCourse);
        }
    }
    public class CourseInfo
    {
        public string CourseName { get; set; }
        public string Department { get; set; }
        public int CourseNumber { get; set; }
        public bool TeachesClass { get; set; }
    }
}
