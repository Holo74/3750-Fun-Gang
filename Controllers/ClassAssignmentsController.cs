using Assignment_1.Data;
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
          
            //var ClassID = HttpContext.Session.GetInt32("ClassID");
           // var Assignments = from a in _context.ClassAssignments select a;
            return View();
        }
        public IActionResult Create() 
        {
            var AssignmentID = HttpContext.Session.GetInt32("AssignmentID");
            return View();
        }
    }
}
