using Assignment_1.Data;
using Assignment_1.Migrations;
using Assignment_1.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace Assignment_1.Controllers
{
    public class ClassAssignmentsController : Controller
    {
        private readonly Assignment_1Context _context;
        public ClassAssignmentsController(Assignment_1Context context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index(int id)
        {

            if (Id != null)
            {
                ClassUserViewModel classUserView = new ClassUserViewModel();
                var user = _context.User.Where(x => x.Id == Id).First();
                classUserView.viewUser = user;
                var UserID = HttpContext.Session.GetInt32("UserID");
                ViewData["Student"] = user.UserType;
                var Course = from c in _context.Class select c;// gets the class table from the database **(still need to show only that specific teacher's courses)**
                if (UserID != null)
                {
                    Course = Course.Where(c => c.UserId == UserID);
                    classUserView.classes = Course.ToList();
                    return View(classUserView);
                }
            }
            return View();
        }
        public IActionResult Create() 
        {
            var AssignmentID = HttpContext.Session.GetInt32("AssignmentID");
            return View();
        }
    }
}
