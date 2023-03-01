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

            CUA.Assignments = (from a in _context.ClassAssignments where a.ClassId == CUA.Class.ClassId select a).ToList();
          //  CUA.Assignments = _context.ClassAssignments.Where(x => x.ClassId == CUA.Class.ClassId).ToList();

            return View(CUA);
        }
        public IActionResult Create(int classId)
        {
            HttpContext.Session.SetInt32("ClassId", classId);


            var courses = from c in _context.Class select c;
            var course = courses.Where(c => c.ClassId == classId).ToList();
            return View(); // just brings the create page up

        }
        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind("ID,ClassId,AssignmentTitle,Description,MaxPoints,DueDate,DueTime,SubmissionType")] ClassAssignments ca)
        {   

            var ClassId = HttpContext.Session.GetInt32("ClassId");
            if (ClassId != null)
            {
                ca.ClassId = ClassId.Value;
                _context.ClassAssignments.Add(ca); 
                await _context.SaveChangesAsync();
            }
            return Redirect(string.Format("/Course?classId={0}",ClassId));//return to index page after creating page
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
