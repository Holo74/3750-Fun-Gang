using System;
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

namespace Assignment_1.Controllers
{
    public class UsersController : Controller
    {
        private readonly Assignment_1Context _context;
        private readonly IWebHostEnvironment _iwebHost;

        public UsersController(Assignment_1Context context, IWebHostEnvironment iwebHost)
        {
            _context = context;
            _iwebHost = iwebHost;

        }

        // GET: Users
        public async Task<IActionResult> Index(int? Id)
        {


            if (Id != null)
            {
                ClassUserViewModel classUserView= new ClassUserViewModel();
                // list constructor default sucks, has to use this
                classUserView.todoitems = new List<TODOitem>();
                var user = _context.User.Where(x => x.Id == Id).First();
                classUserView.viewUser= user;
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




                    Course = from c in _context.Class select c;

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
                    foreach(var x in myassignments)
                    {
                        DateTime dt = new DateTime(x.DueDate.Value.Year, x.DueDate.Value.Month, x.DueDate.Value.Day, x.DueTime.Value.Hour, x.DueTime.Value.Minute, x.DueTime.Value.Second);
                        if(dt > DateTime.Now)
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
                        if(breakint == 5)
                        {
                            break;
                        }
                    }

                    //classUserView.assignments = AssignmentsTODO.ToList();


                    //Registration = Registration.Where(r => r.UserFK == UserID);
                    //classUserView.registrations = Registration.ToList();
                    return View(classUserView);
                }

            }
            return View();
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
        /*
        //GET Users/Details/X
		public async Task<IActionResult> Validate(int? id)
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

			return Redirect("/Users/Details/" + id);
		}
        */
		// GET: Users/Create
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
                _context.Add(user);
                await _context.SaveChangesAsync();
             //   return RedirectToAction(nameof(Index));
            //}
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

            return View(user);
        }


        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,FirstName,LastName,BirthDate,UserType,Address,City,State,ZipCode,PhoneNumber,ReferenceOne,ReferenceTwo,ReferenceThree,Image")]User user)
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            string directory = "wwwroot/Images";
            Directory.CreateDirectory(directory);

            IFormFile z = Request.Form.Files[0];
            string fileExtension = z.FileName.Substring(z.FileName.LastIndexOf('.'));
            string path = directory + "/" + UserID + fileExtension;
            using (Stream filestream = new FileStream(directory + "/" + UserID + fileExtension, FileMode.Create, FileAccess.Write))
            {
                // Saves the file to where ever the filestream was pointed to.
                z.CopyTo(filestream);
                // Won't save properly without this
                filestream.Close();
            }
          
            Console.WriteLine(ModelState.IsValid);
            Console.WriteLine("Model State is valid?");
            Console.WriteLine(ModelState.ErrorCount);
            var olduser = _context.User.Where(x => x.Id == id).FirstOrDefault();

            try
                {
                user.Image = path;
                user.Password = olduser.Password;
                user.UserType = olduser.UserType;        
                user.ConfirmPassword = olduser.Password;
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
                //return RedirectToAction(nameof(Index));
           // }
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
}
