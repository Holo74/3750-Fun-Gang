﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment_1.Data;
using Assignment_1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Assignment_1.Migrations;
using System.IO.Pipes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Threading.Tasks.Dataflow;
using Microsoft.VisualBasic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stripe;

namespace Assignment_1.Controllers
{
    public class UsersController : Controller
    {
        private readonly Assignment_1Context _context;
        private readonly IWebHostEnvironment _iwebHost;
        private IMemoryCache _cache;

        public UsersController(Assignment_1Context context, IWebHostEnvironment iwebHost, IMemoryCache cache)
        {
            _context = context;
            _iwebHost = iwebHost;
            _cache = cache;
        }

        // GET: Users
        public async Task<IActionResult> Index(int? Id)
        {
            if (CacheKeys.UserView != null)
            {
                _cache.TryGetValue(CacheKeys.UserView, out ClassUserViewModel view);

                if (view == null || view.classes[0] == null)
                {
                    ClassUserViewModel classUserView = new ClassUserViewModel();

                    classUserView.todoitems = new List<TODOitem>();
                    var user = _context.User.Where(x => x.Id == Id).First();
                    classUserView.viewUser = user;
                    var UserID = HttpContext.Session.GetInt32("UserID");
                    ViewData["Student"] = user.UserType;
                    var Course = from c in _context.Class select c;
                    var Registration = from r in _context.Registrations select r;// gets the class table from the database **(still need to show only that specific teacher's courses)**
                    if (UserID != null)
                    {
                        if (user.UserType == "Student")
                        {
                            Registration = Registration.Where(r => r.UserFK == UserID);
                            Registration = Registration.Where(r => r.IsRegistered == 1);
                            Course = from Class in Course
                                     join r in Registration on Class.ClassId equals r.ClassFK
                                     select Class;
                        }
                        else
                        {
                            Course = Course.Where(c => c.UserId == UserID);

                        }
                        classUserView.classes = Course.ToList();


                        List<ClassAssignments> myassignments = new List<ClassAssignments>();
                        if (Course != null)
                        {
                            foreach (var mycourse in Course.ToList())
                            {
                                var z = _context.ClassAssignments.Where(y => y.ClassId == mycourse.ClassId).ToList();
                                myassignments.AddRange(z.ToList());
                            }
                        }
                        

                        List<ClassAssignments> futureAssignmentList = new List<ClassAssignments>();
                        foreach (var x in myassignments)
                        {
                            DateTime dt = new DateTime(x.DueDate.Value.Year, x.DueDate.Value.Month, x.DueDate.Value.Day, x.DueTime.Value.Hour, x.DueTime.Value.Minute, x.DueTime.Value.Second);
                            if (dt > DateTime.Now)
                            {
                                futureAssignmentList.Add(x);
                            }

                        }
                        futureAssignmentList = futureAssignmentList.OrderBy(y => y.DueDate.Value.DayOfYear).OrderBy(z => z.DueTime.Value.Date.TimeOfDay.Hours).ToList();

                        var Assignments = _context.ClassAssignments;

                        int breakint = 0;
                        foreach (var assignment in futureAssignmentList)
                        {

                            TODOitem todo = new TODOitem();
                            todo.ID = assignment.Id;
                            todo.AssignmentTitle = assignment.AssignmentTitle;
                            todo.dueDate = assignment.DueDate;
                            todo.dueTime = assignment.DueTime;
                            //var minute = todo.dueTime.Value.Minute;
                            var classList = _context.Class.Where(x => x.ClassId == assignment.ClassId).ToList();
                            if (classList.Count > 0 && classList.Count < 2)
                            {
                                todo.CourseNumber = classList[0].CourseNumber;
                            }

                            classUserView.todoitems.Add(todo);
                            breakint++;
                            if (breakint == 5)
                            {
                                break;
                            }
                        }
                        classUserView.todoitems.OrderBy(x => x.dueDate);
                        classUserView.notifications = GetNotifications(classUserView.viewUser).Result;

                        CacheKeys.UserView = classUserView;
                        _cache.Set(CacheKeys.UserView, classUserView);
                        return View(classUserView);
                    }
                }
                else
                {
                    DateTime date = new DateTime(view.todoitems[0].dueDate.Value.Year, view.todoitems[0].dueDate.Value.Month, view.todoitems[0].dueDate.Value.Day,
                    view.todoitems[0].dueTime.Value.Hour, view.todoitems[0].dueTime.Value.Minute, view.todoitems[0].dueTime.Value.Second);

                    if (date < DateTime.Now)
                    {
                        ClassUserViewModel classUserView = new ClassUserViewModel();
                        
                        classUserView.todoitems = new List<TODOitem>();
                        var user = _context.User.Where(x => x.Id == Id).First();
                        classUserView.viewUser = user;
                        var UserID = HttpContext.Session.GetInt32("UserID");
                        ViewData["Student"] = user.UserType;
                        var Course = from c in _context.Class select c;
                        var Registration = from r in _context.Registrations select r;// gets the class table from the database **(still need to show only that specific teacher's courses)**
                        if (UserID != null)
                        {
                            if (user.UserType == "Student")
                            {
                                Registration = Registration.Where(r => r.UserFK == UserID);
                                Registration = Registration.Where(r => r.IsRegistered == 1);
                                Course = from Class in Course
                                         join r in Registration on Class.ClassId equals r.ClassFK
                                         select Class;
                            }
                            else
                            {
                                Course = Course.Where(c => c.UserId == UserID);

                            }
                            classUserView.classes = Course.ToList();

                            List<ClassAssignments> myassignments = new List<ClassAssignments>();
                            foreach (var mycourse in Course.ToList())
                            {
                                var z = _context.ClassAssignments.Where(y => y.ClassId == mycourse.ClassId).ToList();
                                myassignments.AddRange(z.ToList());
                            }

                            List<ClassAssignments> futureAssignmentList = new List<ClassAssignments>();
                            foreach (var x in myassignments)
                            {
                                DateTime dt = new DateTime(x.DueDate.Value.Year, x.DueDate.Value.Month, x.DueDate.Value.Day, x.DueTime.Value.Hour, x.DueTime.Value.Minute, x.DueTime.Value.Second);
                                if (dt > DateTime.Now)
                                {
                                    futureAssignmentList.Add(x);
                                }

                            }
                            futureAssignmentList = futureAssignmentList.OrderBy(y => y.DueDate.Value.DayOfYear).OrderBy(z => z.DueTime.Value.Date.TimeOfDay.Hours).ToList();
                            
                            var Assignments = _context.ClassAssignments;

                            int breakint = 0;
                            foreach (var assignment in futureAssignmentList)
                            {

                                TODOitem todo = new TODOitem();
                                todo.ID = assignment.Id;
                                todo.AssignmentTitle = assignment.AssignmentTitle;
                                todo.dueDate = assignment.DueDate;
                                todo.dueTime = assignment.DueTime;
                                //var minute = todo.dueTime.Value.Minute;
                                var classList = _context.Class.Where(x => x.ClassId == assignment.ClassId).ToList();
                                if (classList.Count > 0 && classList.Count < 2)
                                {
                                    todo.CourseNumber = classList[0].CourseNumber;
                                }

                                classUserView.todoitems.Add(todo);
                                breakint++;
                                if (breakint == 5)
                                {
                                    break;
                                }
                            }
                            classUserView.todoitems.OrderBy(x => x.dueDate);
                            classUserView.notifications = GetNotifications(classUserView.viewUser).Result;

                            CacheKeys.UserView = classUserView;
                            _cache.Set(CacheKeys.UserView, classUserView);
                            return View(classUserView);
                        }
                    }
                }
                ViewData["Student"] = view.viewUser.UserType;
                return View(view);
            }
            else if (Id != null)
            {
                ClassUserViewModel classUserView = new ClassUserViewModel();
                // list constructor default sucks, has to use this
                classUserView.todoitems = new List<TODOitem>();
                var user = _context.User.Where(x => x.Id == Id).First();
                classUserView.viewUser = user;
                var UserID = HttpContext.Session.GetInt32("UserID");
                ViewData["Student"] = user.UserType;
                var Course = from c in _context.Class select c;
                var Registration = from r in _context.Registrations select r;// gets the class table from the database **(still need to show only that specific teacher's courses)**
                if (UserID != null)
                {
                    if (user.UserType == "Student")
                    {
                        Registration = Registration.Where(r => r.UserFK == UserID);
                        Registration = Registration.Where(r => r.IsRegistered == 1);
                        Course = from Class in Course
                                 join r in Registration on Class.ClassId equals r.ClassFK
                                 select Class;

                        //_context.Registrations.Where(y => y.)
                        //Course = t;
                    }
                    else
                    {
                        Course = Course.Where(c => c.UserId == UserID);

                    }
                    classUserView.classes = Course.ToList();


                    //step 1: Find out all classes for this student, student id.

                    // step 2: run a loop, and find all assignemnts for each class and accumulate them. 

                    List<ClassAssignments> myassignments = new List<ClassAssignments>();
                    foreach (var mycourse in Course.ToList())
                    {
                        //
                        //mycourse.ClassId
                        //
                        //mycourse.DueDate > today.date
                        var z = _context.ClassAssignments.Where(y => y.ClassId == mycourse.ClassId).ToList();
                        myassignments.AddRange(z.ToList());


                    }

                    List<ClassAssignments> futureAssignmentList = new List<ClassAssignments>();
                    foreach (var x in myassignments)
                    {
                        DateTime dt = new DateTime(x.DueDate.Value.Year, x.DueDate.Value.Month, x.DueDate.Value.Day, x.DueTime.Value.Hour, x.DueTime.Value.Minute, x.DueTime.Value.Second);
                        if (dt > DateTime.Now)
                        {
                            futureAssignmentList.Add(x);
                        }

                    }
                    futureAssignmentList = futureAssignmentList.OrderBy(y => y.DueDate.Value.DayOfYear).OrderBy(z => z.DueTime.Value.Date.TimeOfDay.Hours).ToList();
                    //futureAssignmentList = futureAssignmentList.OrderBy(y => y.DueTime.Value.Hour).ToList();
                    //futureAssignmentList = futureAssignmentList.OrderBy(y => y.DueTime.Value.Minute).ToList();
                    //futureAssignmentList = futureAssignmentList.OrderBy(y => y.DueTime.Value.Second).ToList();
                    // step 3: sort them by due date and pick the top 5. 

                    var Assignments = _context.ClassAssignments;

                    //               myassignments = myassignments.Where(a => a.DueDate >= DateTime.Today).ToList();
                    //myassignments = myassignments.Where(a => a.DueTime.Value.Hour >= DateTime.Today.Hour).ToList();
                    //myassignments = myassignments.Where(a => a.DueTime.Value.Minute >= DateTime.Today.Minute).ToList();
                    //myassignments = myassignments.Where(a => a.DueTime.Value.Second >= DateTime.Today.Second).ToList();

                    //var AssignmentList = Assignments.ToList();
                    int breakint = 0;
                    foreach (var assignment in futureAssignmentList)
                    {

                        TODOitem todo = new TODOitem();
                        todo.ID = assignment.Id;
                        todo.AssignmentTitle = assignment.AssignmentTitle;
                        todo.dueDate = assignment.DueDate;
                        todo.dueTime = assignment.DueTime;
                        //var minute = todo.dueTime.Value.Minute;
                        var classList = _context.Class.Where(x => x.ClassId == assignment.ClassId).ToList();
                        if (classList.Count > 0 && classList.Count < 2)
                        {
                            todo.CourseNumber = classList[0].CourseNumber;
                        }

                        classUserView.todoitems.Add(todo);
                        breakint++;
                        if (breakint == 5)
                        {
                            break;
                        }
                    }

                    //classUserView.assignments = AssignmentsTODO.ToList();
                    classUserView.todoitems.OrderBy(x => x.dueDate);
                    classUserView.notifications = GetNotifications(classUserView.viewUser).Result;

                    //Registration = Registration.Where(r => r.UserFK == UserID);
                    //classUserView.registrations = Registration.ToList();
                    CacheKeys.UserView = classUserView;
                    _cache.Set(CacheKeys.UserView, classUserView);
                    return View(classUserView);
                }
            }
            return View();
        }

        public async Task<List<string>> GetNotifications(User viewUser)
        {
            List<string> notification = new List<string>();
            var joinedAssignmentTables =
            from submitted in _context.AssignmentSubmissions
            join assignment in _context.ClassAssignments on
            submitted.AssignmentFK equals assignment.Id
            join assignClass in _context.Class on
            submitted.ClassFK equals assignClass.ClassId
            where
            submitted.Modified > viewUser.LastedLoggedIn
            where
            submitted.UserFK == viewUser.Id
            select assignment.AssignmentTitle + " in class " + assignClass.CourseName + " was graded";

            notification = notification.Concat(joinedAssignmentTables).ToList();

            var joinedClassCreatedAssignment =
                from assignments in _context.ClassAssignments
                join assignClass in _context.Class on
                assignments.ClassId equals assignClass.ClassId
                join registClass in _context.Registrations on
                assignClass.ClassId equals registClass.ClassFK
                where registClass.UserFK == viewUser.Id
                where assignments.CreatedDate > viewUser.LastedLoggedIn
                select assignClass.CourseName + " Has created a new assignment named " + assignments.AssignmentTitle;

            notification = notification.Concat(joinedClassCreatedAssignment).ToList();

            User updatingLastLogin = _context.User.First(x => x.Id == viewUser.Id);

            // Note to self. Change the datetime now cause that uses the local systems time not the servers time
            updatingLastLogin.LastedLoggedIn = DateTime.Now;
            _context.User.Update(updatingLastLogin);
            await _context.SaveChangesAsync();

            return notification;
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details()//int? id)
        {
            UserView uv = new UserView();
            var UserID = HttpContext.Session.GetInt32("UserID");

            if (UserID == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == UserID);
            if (user == null)
            {
                return NotFound();
            }
            uv.PhoneNumber = user.PhoneNumber;
            uv.FirstName = user.FirstName;
            uv.LastName = user.LastName;
            uv.Address = user.Address;  
            uv.City = user.City;
            uv.State = user.State;
            uv.ZipCode = user.ZipCode;
            uv.BirthDate = user.BirthDate;
            uv.Balance  = user.Balance;
            uv.Image = user.Image;
            uv.ReferenceOne = user.ReferenceOne;
            uv.ReferenceTwo = user.ReferenceTwo;
            uv.ReferenceThree  = user.ReferenceThree;
            uv.ConfirmPassword = user.ConfirmPassword;
            uv.Email = user.Email;
            uv.Password = user.Password;
            uv.UserType = user.UserType;
            uv.ShowImage = TransformImagePath(user.Image);

            ViewData["Student"] = user.UserType;
            return View(uv);
        }

        private string TransformImagePath (string? imagePath)
        {
            if (imagePath != null)
            {
                string newString = string.Empty;
                System.Diagnostics.Debug.WriteLine(imagePath);
                //newString = imagePath.Remove"../.." 
                newString = imagePath.Replace("wwwroot", "../..");
                return newString;
            }
            return string.Empty;
        }
        public async Task<IActionResult> ViewAll(){
            var users = from u in _context.User
                        select u;
            //something null inside database this doesnt like
            //var users = _context.User.Where(x => x.Id == 17);

            var temp = await users.FirstOrDefaultAsync();
            return View(users);
        }
        
		public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Password,FirstName,LastName,BirthDate,ConfirmPassword,UserType")] User user)
        {
            //TODO: Fix this. We need model state validation before the meeting. 
           // if (ModelState.IsValid)
            //{
            //
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(new Models.User(), user.Password);//hash before sending to database
                user.Balance = 0;
                _context.Add(user);
                await _context.SaveChangesAsync();
            
            return Redirect("/Login/");
        }

        // GET: Users/Edit/5
        
        public async Task<IActionResult> Edit()//(int? id)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            
            if (UserID == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(UserID);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Student"] = user.UserType;
            return View(user);
        }


        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,FirstName,LastName,BirthDate,UserType,Address,City,State,ZipCode,PhoneNumber,ReferenceOne,ReferenceTwo,ReferenceThree,Image")]User user)
        {

            int i;
            var UserID = HttpContext.Session.GetInt32("UserID");
            string directory = "wwwroot/Images";
            Directory.CreateDirectory(directory);
            
            
          
            Console.WriteLine(ModelState.IsValid);
            Console.WriteLine("Model State is valid?");
            Console.WriteLine(ModelState.ErrorCount);
            var olduser = _context.User.Where(x => x.Id == id).FirstOrDefault();
            if (Request.Form.Files.Count > 0)
            {
                
                IFormFile z = Request.Form.Files[0];
                string fileExtension = z.FileName.Substring(z.FileName.LastIndexOf('.'));
                string path = directory + "/" + z.FileName;
                using (Stream filestream = new FileStream(directory + "/" + z.FileName, FileMode.Create, FileAccess.Write))
                {
                    // Saves the file to where ever the filestream was pointed to.
                    z.CopyTo(filestream);
                    // Won't save properly without this
                    filestream.Close();
                    user.Image = path;
                }
            }
            else
            {
                user.Image = olduser.Image;
            }

            try
            {
                
                user.Password = olduser.Password;
                user.UserType = olduser.UserType;        
                user.ConfirmPassword = olduser.Password;
                user.Balance = olduser.Balance;
                _context.ChangeTracker.Clear();
                _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            ViewData["Student"] = user.UserType;
            return Redirect("/Users/Details/" + user.Id);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.User == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.User == null)
            {
                return Problem("Entity set 'Assignment_1Context.User'  is null.");
            }
            var user = await _context.User.FindAsync(id);
            if (user != null)
            {
                _context.User.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return _context.User.Any(e => e.Id == id);
        }
    }
    public class UserView
    {
        public int Id { get; set; }

        //[RegularExpression(@"^[a-zA-Z\.]*+@+[a-zA-Z\.]*$")]
        [Required]
        public string Email { get; set; }

        // Requirements.  A single lower cases letter.  1 upper case letter.  1 decimal.  At least 1 special character.  Min length of 8 characters
        [Validators.PasswordValidation]
        //[StringLength(80, MinimumLength = 6)]
        [Required]
        public string Password { get; set; }
        [Compare(otherProperty: "Password"), Display(Name = "Confirm Password"), NotMapped]
        public string ConfirmPassword { get; set; }

        // [Required]
        public string FirstName { get; set; }
        //[Required]
        public string LastName { get; set; }

        [DateValidation(ErrorMessage = "User Age must be at least 16")]

        //[Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        //  [Required]
        public string UserType { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? PhoneNumber { get; set; }

        //[DataType(DataType.Currency)]
        public decimal? Balance { get; set; }
        public string? ReferenceOne { get; set; }
        public string? ReferenceTwo { get; set; }
        public string? ReferenceThree { get; set; }
        //this might not work
        public string? Image { get; set; } = "";
        public string? ShowImage { get; set; } = "";
        //public string? Image { get; set; } = "";
    }
    public static class CacheKeys
    {
        public static ClassUserViewModel? UserView { get; set; }
    }
}
