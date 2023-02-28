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

        public IActionResult Assignment(int ID)
        {
            AssignmentSubmissionViewModel ASVM = new AssignmentSubmissionViewModel();            

            //set assignment according to ID
            var assignments = from a in _context.ClassAssignments select a;
            ASVM.Assignment = assignments.Where(a => a.Id == ID).FirstOrDefault();

            //check database for existing entry
            int? UserID = HttpContext.Session.GetInt32("UserID");
            int ClassID = ASVM.Assignment.ClassId;
            HttpContext.Session.SetInt32("AssignmentID",ClassID);//just in case
            int AssignmentID = ID;
            var submission = from s in _context.AssignmentSubmissions select s;
            if (UserID != null && ASVM.Assignment != null)
            {
                //find this users assignment for a class if it exists
                ASVM.Submission = submission.Where(x => x.UserFK== UserID).Where(y => y.AssignmentFK == AssignmentID).Where(z => z.ClassFK == ClassID).FirstOrDefault();
                
            }
            return View(ASVM);
        }

        public IActionResult Submit()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Submit([Bind("Id,Data")] AssignmentSubmissions s)
        {
            //AssignmentSubmissions s = new AssignmentSubmissions();
            int? UserID = HttpContext.Session.GetInt32("UserID");
            int? AssignmentID = HttpContext.Session.GetInt32("AssignmentID");
            s.UserFK = UserID == null ? 0 : UserID.Value;
            s.AssignmentFK = AssignmentID == null ? 0 : AssignmentID.Value;
            s.ClassFK = 7;
            //s.Data= Data;
            s.SubmitDate= DateTime.Now;
            //s.SubmitTime= default(DateTime).Add(DateTime.Now.TimeOfDay);

			_context.AssignmentSubmissions.Add(s);
			await _context.SaveChangesAsync();

			return RedirectToAction("Assignment", new {ID = 7});//hardcode for testing
        }
    }
}
