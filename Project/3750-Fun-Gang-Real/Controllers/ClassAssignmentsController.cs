﻿using Assignment_1.Data;
using Assignment_1.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Drawing;
using Microsoft.Extensions.Caching.Memory;

namespace Assignment_1.Controllers
{
    public class ClassAssignmentsController : Controller
    {
        private readonly Assignment_1Context _context;
        private IMemoryCache _cache;
        public ClassAssignmentsController(Assignment_1Context context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        [HttpGet]
        public IActionResult Index(int id)
        {

            //if (id != null)
            //{
            //    ClassUserAssignments classUserAssignmentView = new ClassUserAssignments();
            var user = _context.User.Where(x => x.Id == id).First();
            //    classUserAssignmentView.User = user;
            //    var UserID = HttpContext.Session.GetInt32("UserID");
            ViewData["Student"] = user.UserType;
            //    var Course = from c in _context.Class select c;
            //    if (UserID != null)
            //    {
            //        Course = Course.Where(c => c.UserId == UserID);
            //        classUserAssignmentView.Class = Course.ToList();
            //        return View(classUserAssignmentView);
            //    }
            //}



            return View();
        }
        public IActionResult Create() 
        {
            var AssignmentID = HttpContext.Session.GetInt32("AssignmentID");
            int? UserID = HttpContext.Session.GetInt32("UserID");
            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();
            ViewData["Student"] = user[0].UserType;
            return View();
        }
       public async Task<int> CreateMain(int id, ClassAssignments ca)
        {
            ca.ClassId = id == null ? -1 : id;
            ca.CreatedDate = DateTime.Now;
            _context.ClassAssignments.Add(ca);
            
            await _context.SaveChangesAsync();
            return 1;


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

            //-------------< gets all assignments for the chart >--------------
            var l = from k in _context.AssignmentSubmissions select k;
            ASVM.Submission = l.Where(x => x.AssignmentFK == ID).ToList();

            int maxPoints = 100;
            if (ASVM.Assignment != null)
            {
                maxPoints = ASVM.Assignment.MaxPoints.Value;
            }

            foreach (var item in ASVM.Submission)
            {

                int t = item.Points == null ? 0 : item.Points.Value;//get point value if it exists

                //get grade fraction, multiply by 10 (90/100 -> 0.9 -> 9.0) and cast to int, use this value as index of asvm grade bin and incriment it
                t = (int)(10 * ((float)t / (float)maxPoints));
                if (t <= 10)
                {
                    ASVM.GradeBins[t]++;
                }
                else
                {
                    //if it gets here then the points earned is > 100% of possible points
                    ASVM.GradeBins[10]++;
                }
                
            }
            //-----------------------------------------------------------------
            //in case not already set, used in the submit()
            HttpContext.Session.SetInt32("AssignmentID",ID);
            HttpContext.Session.SetInt32("ClassID",ClassID);
            //ASVM.GradeBins = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6 };

            int AssignmentID = ID;
            var submission = from s in _context.AssignmentSubmissions select s;
            if (UserID != null && ASVM.Assignment != null)
            {
                //find this users assignment for a class if it exists
                ASVM.Submission = submission.Where(x => x.UserFK== UserID).Where(y => y.AssignmentFK == AssignmentID).Where(z => z.ClassFK == ClassID);
                
            }

            ASVM.StudentBin = 0;
            if(ASVM.Submission != null && ASVM.Submission.Count() > 0) {

                if(ASVM.Submission.First().Points != null)
                {
                    ASVM.StudentBin = (int)(10 * ((float)ASVM.Submission.First().Points / (float)maxPoints));
                }
            }

            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();
            ViewData["Student"] = user[0].UserType;

            return View(ASVM);
        }

        public IActionResult Submit()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Submit([Bind("Id,Data,SubmissonType")] AssignmentSubmissions s)
        {
            //AssignmentSubmissions s = new AssignmentSubmissions();
            int? UserID = HttpContext.Session.GetInt32("UserID");
            int? AssignmentID = HttpContext.Session.GetInt32("AssignmentID");
            int? ClassID = HttpContext.Session.GetInt32("ClassID");
            s.UserFK = UserID == null ? 0 : UserID.Value;
            s.AssignmentFK = AssignmentID == null ? 0 : AssignmentID.Value;
            s.ClassFK = ClassID == null ? 0 : ClassID.Value;
            //s.Data= Data;
            s.SubmitDate= DateTime.Now;
            //s.SubmitTime= default(DateTime).Add(DateTime.Now.TimeOfDay);

            if (s.SubmissonType == "Text" || s.SubmissonType == "text")
            {
                _context.AssignmentSubmissions.Add(s);
                await _context.SaveChangesAsync();
            }
            else
            {
                string directory = "wwwroot/Submissions";
                Directory.CreateDirectory(directory);
                IFormFile z = Request.Form.Files[0];
                string fileExtension = z.FileName.Substring(z.FileName.LastIndexOf('.'));
                string path = directory + "/" + UserID + "." + AssignmentID + fileExtension;
                using (Stream filestream = new FileStream(directory + "/" + UserID + "." + AssignmentID + fileExtension, FileMode.Create, FileAccess.Write))
                {
                    // Saves the file to where ever the filestream was pointed to.
                    z.CopyTo(filestream);
                    // Won't save properly without this
                    filestream.Close();
                }

                s.Data = TransformSubPath(path);

                _context.AssignmentSubmissions.Add(s);
                await _context.SaveChangesAsync();
            }
                return RedirectToAction("Assignment", new { ID = AssignmentID });//hardcode for testing
            
            }

            public IActionResult Submissions(int ID)//ID is the id of an assignment
        {
			AssignmentSubmissionViewModel ASVM = new AssignmentSubmissionViewModel();
            var assignments = from b in _context.ClassAssignments select b;
            ASVM.Assignment = assignments.Where(y => y.Id== ID).First();
            var s = from a in _context.AssignmentSubmissions select a;
            ASVM.Submission = s.Where(x => x.AssignmentFK == ID).ToList();

            int maxPoints = 100;
            if(ASVM.Assignment != null)
            {
                maxPoints = ASVM.Assignment.MaxPoints.Value;
            }

            foreach(var item in ASVM.Submission)
            {
                
                int t = item.Points == null ? 0 : item.Points.Value;//get point value if it exists

				//get grade fraction, multiply by 10 (90/100 -> 0.9 -> 9.0) and cast to int, use this value as index of asvm grade bin and incriment it
				t = (int)(10 * ((float)t / (float)maxPoints));
                if (t <= 10)
                {
                    ASVM.GradeBins[t]++;
                }
                else
                {
                    //if it gets here then the points earned is > 100% of possible points
                    ASVM.GradeBins[10]++;
                }
            }

            // Added all this list code to get the users because I can't use the entire database (crashes)
            // might still crash unless we fix the problem in the users table
            List<int> IDs = new List<int>();

            for (int i = 0; i < ASVM.Submission.Count(); i++)
            {
                if (!IDs.Contains(ASVM.Submission.ElementAt(i).UserFK)){
                    IDs.Add(ASVM.Submission.ElementAt(i).UserFK); // adds IDs that are not already in the list
                }
            }

            List<User> Users = new List<User>();
            for (int i = 0; i < IDs.Count(); i++)
            {
                // adds Users who submitted something to table one by one
                Users.Add(_context.User.Where(u => u.Id == IDs.ElementAt(i)).First());
            }

            ASVM.User = Users;

            int? UserID = HttpContext.Session.GetInt32("UserID");
            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();
            ViewData["Student"] = user[0].UserType;

            return View(ASVM);
        }
        private string TransformSubPath(string? subPath)
        {
            if(subPath != null)
            {
                string newString = string.Empty;
                System.Diagnostics.Debug.WriteLine(subPath);
                newString = subPath.Replace("wwwroot", "../..");
                return newString;

            }
            return string.Empty;
        }
        public IActionResult Grade(int ID)//submission id
        {
            AssignmentSubmissionViewModel ASVM = new AssignmentSubmissionViewModel();
            var sub = from s in _context.AssignmentSubmissions select s;
            ASVM.Submission = sub.Where(x => x.Id == ID);
            var asn = from a in _context.ClassAssignments select a;
            ASVM.Assignment = asn.Where(y => y.Id == ASVM.Submission.First().AssignmentFK).First();

            int? UserID = HttpContext.Session.GetInt32("UserID");
            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();
            ViewData["Student"] = user[0].UserType;

            return View(ASVM);
        }

        [HttpGet]
        public IActionResult SetPoints()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> SetPoints(int points, int ID)
		{
            var s = from a in _context.AssignmentSubmissions select a;
            AssignmentSubmissions t = s.Where(x => x.Id== ID).First();
            t.Points = points;
            _context.Update(t);
            await _context.SaveChangesAsync();
			return RedirectToAction("Submissions", new { ID = t.AssignmentFK });
		}

   //     public async Task<IActionResult> Details(int? id)
   //     {
   //         if (id == null || _context.ClassAssignments == null)
   //         {
   //             return NotFound();
   //         }

   //         var courseA = await _context.ClassAssignments
   //             .FirstOrDefaultAsync(m => m.Id == id);
   //         if (courseA == null)
   //         {
   //             return NotFound();
   //         }

   //         return View(courseA);
   //     }
   //     public async Task<IActionResult> Edit(int? id)
   //     {


   //         if (id == null || _context.ClassAssignments == null)
   //         {
   //             return NotFound();
   //         }
   //         ///ca = ClassAssignments
			//var ca = await _context.ClassAssignments.FindAsync(id);
   //         if (ca == null)
   //         {
   //             return NotFound();
   //         }

   //         return View(ca);

   //     }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ID,ClassId,AssignmentTitle,Description,MaxPoints,DueDate,DueTime,SubmissionType")] ClassAssignments ca)
        //{


        //    var Id = HttpContext.Session.GetInt32("ID");

        //    var oldID = _context.ClassAssignments.Where(x => x.Id == id).FirstOrDefault();



        //    _context.ChangeTracker.Clear();
        //    _context.Update(ca);
        //    await _context.SaveChangesAsync();

        //    return Redirect(string.Format("/ClassAssignments/Details/ID={0}", Id));//return to index page after creating page
        //}
        ////      public IActionResult DownloadFile()
        //      {
        //          AssignmentSubmissions asub;
        //          var memory = DownloadSingleFile(asub.Data);
        //      }
        //private MemoryStream DownloadSingleFile(string uploadPath)
        //      {
        //          var path = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);
        //          var memory = new MemoryStream();
        //          if(System.IO.File.Exists(path))
        //          {
        //              var net = new System.Net.WebClient();
        //              var data = net.DownloadData(path);
        //              var content = new System.IO.MemoryStream(data);

        //          }
        //          memory.Position = 0;
        //          return memory;
        //      }
    }
}
