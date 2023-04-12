using Assignment_1.Data;
using Assignment_1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();
            var courses = from c in _context.Class select c;
            var course = courses.Where(c => c.ClassId == classId).ToList();
            var assignments = from d in _context.ClassAssignments select d;
            var assignment = assignments.Where(d => d.ClassId == classId).ToList();
            var submissions = from e in _context.AssignmentSubmissions select e;
            submissions = submissions.Where(e => e.UserFK == UserID);
            var submissionList = submissions.Where(f => f.ClassFK== classId).ToList();

            var studentlist = from reg in _context.Registrations select reg;
            var studentlist2 = (from reg in _context.Registrations select reg).Where(cor => cor.ClassFK== (classId == null ? 0 : classId.Value)).ToList();



			StudentAssignmentListViewModel SALV = new StudentAssignmentListViewModel();

            SALV.Assignments = new List<AssignmentSubmissionViewModel>();

            foreach(var c in assignment)
            {
                AssignmentSubmissionViewModel d = new AssignmentSubmissionViewModel();

                d.Assignment = c;
                d.Submission = submissions.Where(e => e.AssignmentFK == c.Id).ToList();
                SALV.Assignments.Add(d);
            }

            //foreach(var a in submissionList)
            //{
            //    AssignmentSubmissionViewModel b= new AssignmentSubmissionViewModel();

            //    b.Submission = submissions.Where(e => e.AssignmentFK == a.Id).ToList();
            //    b.Submission.Append(a);
            //    b.Assignment= assignments.Where(g => g.Id == a.AssignmentFK).FirstOrDefault();

            //    SALV.Assignments.Add(b);

            //}


            if (course.Count != 1)
            {
                return NotFound();
            }

			SALV.Class = course[0];

            // CourseInfo specificCourse = new CourseInfo();
            // specificCourse.CourseName = course[0].CourseName;
            // specificCourse.CourseNumber = course[0].CourseNumber;
            // specificCourse.Department = course[0].Department;

            // Does User teach the class?
            if (SALV.Class.UserId == UserID)
            {
				SALV.User = Models.Helper.ReturnFirstSelected<User>((from u in _context.User where u.Id == UserID select u).ToList());
				SALV.TeachesClass = true;
            }
            else
            {
                SALV.TeachesClass = false;
            }

			//SALV.Assignments.Add();

            
            //SALV.Assignments = (from a in _context.ClassUserAssignments where a.ClassId == SALV.Class.ClassId select a).ToList();
            //  CUA.Assignments = _context.ClassAssignments.Where(x => x.ClassId == CUA.Class.ClassId).ToList();
            var t = from u in _context.User select u;
            SALV.User = t.Where(t => t.Id == UserID).FirstOrDefault();

            int gradeA = 0;
			int gradeAm = 0;
			int gradeBp = 0;
			int gradeB = 0;
			int gradeBm = 0;
			int gradeCp = 0;
			int gradeC = 0;
			int gradeCm = 0;
			int gradeDp = 0;
			int gradeD = 0;
			int gradeDm = 0;
			int gradeE = 0;
            
            //if (SALV.User.UserType != "Student")
            //{
                foreach(var studentuser in studentlist2.Where(x => x.IsRegistered == 1).ToList())
                {

					var submissionsPerStudent = _context.AssignmentSubmissions.Where(e => e.UserFK == studentuser.UserFK);
					var submissionsPerClass = _context.AssignmentSubmissions.Where(f => f.ClassFK == classId).ToList();
					double totalgrade = 0;
					double totalmaxgrade = 0;
					foreach (var item in submissionsPerStudent)
					{
						var assignmentCurrent = assignment.Where(f => f.Id == item.AssignmentFK).FirstOrDefault();

						if (item.Points == null) continue;
						float currentgrade = ((float)item.Points.Value / (float)(assignmentCurrent?.MaxPoints == null ? 1 : assignmentCurrent.MaxPoints.Value));

						float currentpercent = currentgrade * 100;
                        
				        
                        totalgrade = totalgrade + (double)item.Points;
					    totalmaxgrade = totalmaxgrade + (double)assignmentCurrent.MaxPoints;
						

					}
					double totalpercent = 0;
					totalpercent = (totalgrade / totalmaxgrade) * 100;
					if (totalpercent >= 94)
					{
						gradeA++;
					}
					else if (totalpercent >= 90)
					{
						gradeAm++;
					}
					else if (totalpercent >= 87)
					{
						gradeBp++;
					}
					else if (totalpercent >= 84)
					{
						gradeB++;
					}
					else if (totalpercent >= 80)
					{
						gradeBm++;
					}
					else if (totalpercent >= 77)
					{
						gradeCp++;
					}
					else if (totalpercent >= 74)
					{
						gradeC++;
					}
					else if (totalpercent >= 70)
					{
						gradeCm++;
					}
					else if (totalpercent >= 67)
					{
						gradeDp++;
					}
					else if (totalpercent >= 64)
					{
						gradeD++;
					}
					else if (totalpercent >= 60)
					{
						gradeDm++;
					}
					else
					{
						gradeE++;
					}



					//foreach (var item in SALV.Assignments)
					//               {
					//                   if (item.Submission.Count() == 1)
					//                   {
					//                       if (item.Submission.FirstOrDefault().Points != null)
					//              {

					//	    }
					//                   }
					//               }

				}
				
			//}

   //         if(SALV.User.UserType == "Student")
   //         {
   //             foreach (var grade in submissions)
   //             {
   //                 var isassignment = assignment.Where(f => f.Id == grade.AssignmentFK).First();

   //                 if (grade.Points == null) continue;
   //                 float currentgrade = ((float)grade.Points.Value/(float)(isassignment?.MaxPoints==null ?1:isassignment.MaxPoints.Value));

   //                 float currentpercent = currentgrade * 100;

   //                 if (currentpercent >= 94)
   //                 {
   //                     gradeA++;
   //                 }
   //                 else if (currentpercent >= 90)
   //                 {
   //                     gradeAm++;
   //                 }
   //                 else if (currentpercent >= 87)
   //                 {
   //                     gradeBp++;
   //                 }
   //                 else if (currentpercent >= 84)
   //                 {
   //                     gradeB++;
   //                 }
   //                 else if (currentpercent >= 80)
   //                 {
   //                     gradeBm++;
   //                 }
   //                 else if (currentpercent >= 77)
   //                 {
   //                     gradeCp++;
   //                 }
   //                 else if (currentpercent >= 74)
   //                 {
   //                     gradeC++;
   //                 }
   //                 else if (currentpercent >= 70)
   //                 {
   //                     gradeCm++;
   //                 }
   //                 else if (currentpercent >= 67)
   //                 {
   //                     gradeDp++;
   //                 }
   //                 else if (currentpercent >= 64)
   //                 {
   //                     gradeD++;
   //                 }
   //                 else if (currentpercent >= 60)
   //                 {
   //                     gradeDm++;
   //                 }
   //                 else
   //                 {
   //                     gradeE++;
   //                 }
   //             }
   //         }
			

			SALV.PiechartData = new List<PieChartNameandAmount>();
            SALV.PiechartData.Add(new PieChartNameandAmount() { Name = "A", Amount = gradeA });
            SALV.PiechartData.Add(new PieChartNameandAmount() { Name = "A-", Amount = gradeAm });
			SALV.PiechartData.Add(new PieChartNameandAmount() { Name = "B+", Amount = gradeBp });
			SALV.PiechartData.Add(new PieChartNameandAmount() { Name = "B", Amount = gradeB });
			SALV.PiechartData.Add(new PieChartNameandAmount() { Name = "B-", Amount = gradeBm });
			SALV.PiechartData.Add(new PieChartNameandAmount() { Name = "C+", Amount = gradeCp });
			SALV.PiechartData.Add(new PieChartNameandAmount() { Name = "C", Amount = gradeC });
			SALV.PiechartData.Add(new PieChartNameandAmount() { Name = "C-", Amount = gradeCm });
			SALV.PiechartData.Add(new PieChartNameandAmount() { Name = "D+", Amount = gradeDp });
			SALV.PiechartData.Add(new PieChartNameandAmount() { Name = "D", Amount = gradeD });
			SALV.PiechartData.Add(new PieChartNameandAmount() { Name = "D-", Amount = gradeDm });
			SALV.PiechartData.Add(new PieChartNameandAmount() { Name = "E", Amount = gradeE });


            ViewData["Student"] = user[0].UserType;
            return View(SALV);
        }
        public IActionResult Create(int classId)
        {
            HttpContext.Session.SetInt32("ClassId", classId);


            var courses = from c in _context.Class select c;
            var course = courses.Where(c => c.ClassId == classId).ToList();

            var UserID = HttpContext.Session.GetInt32("UserID");
            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();
            ViewData["Student"] = user[0].UserType;

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

        public async Task<IActionResult> Edit(int? id)
        {


            if (id == null || _context.ClassAssignments == null)
            {
                return NotFound();
            }
            ///ca = ClassAssignments
			var ca = await _context.ClassAssignments.FindAsync(id);
            if (ca == null)
            {
                return NotFound();
            }
            var UserID = HttpContext.Session.GetInt32("UserID");
            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();
            ViewData["Student"] = user[0].UserType;
            return View(ca);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClassId,AssignmentTitle,Description,MaxPoints,DueDate,DueTime,SubmissionType")] ClassAssignments ca)
        {
            //var ClassId = _context.ClassAssignments.Where(x => x.Id == id).FirstOrDefault();


            //var oldID = _context.ClassAssignments.Where(x => x.Id == id).FirstOrDefault();

            var UserID = HttpContext.Session.GetInt32("UserID");
            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();
            ViewData["Student"] = user[0].UserType;

            _context.ChangeTracker.Clear();
            _context.Update(ca);
            await _context.SaveChangesAsync();

            return Redirect("/Course?classId="+ ca.ClassId);
        }
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || _context.ClassAssignments == null)
			{
				return NotFound();
			}

			var ai = await _context.ClassAssignments
				.FirstOrDefaultAsync(m => m.Id == id);
			if (ai == null)
			{
				return NotFound();
			}

            var UserID = HttpContext.Session.GetInt32("UserID");
            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();
            ViewData["Student"] = user[0].UserType;

            return View(ai);
		}
		[HttpPost, ActionName("DeleteC")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id, ClassAssignments ca)
		{
			var a = await _context.ClassAssignments.FindAsync(id);
			_context.ClassAssignments.Remove(ca);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));

		
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
