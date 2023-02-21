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

namespace Assignment_1.Controllers
{
    public class UsersController : Controller
    {
        private readonly Assignment_1Context _context;

        public UsersController(Assignment_1Context context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(int? Id)
        {
            if (Id != null)
            {
                ClassUserViewModel classUserView= new ClassUserViewModel();
                var user = _context.User.Where(x => x.Id == Id).First();
                classUserView.viewUser= user;
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

        // GET: Users/Details/5
        public async Task<IActionResult> Details()//int? id)
        {
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

            return View(user);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,FirstName,LastName,BirthDate,UserType,Address,City,State,ZipCode,PhoneNumber,ReferenceOne,ReferenceTwo,ReferenceThree")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }
            Console.WriteLine(ModelState.IsValid);
            Console.WriteLine("Model State is valid?");
            Console.WriteLine(ModelState.ErrorCount);
            //   if (ModelState.IsValid)
            // {
            var olduser = _context.User.Where(x => x.Id == id).FirstOrDefault();
            try
                {
               
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
}
