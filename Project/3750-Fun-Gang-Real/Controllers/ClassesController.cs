using Assignment_1.Data;
using Assignment_1.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace Assignment_1.Controllers
{
    public class ClassesController : Controller
    {
        private readonly Assignment_1Context _context; // declaration for the context object
        private IMemoryCache _cache;

        public ClassesController(Assignment_1Context context, IMemoryCache cache)
        {
            _context = context; // makes it so we can get the database at any time
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Index()
        {
            /*needs to take a user as input for this query to work
             * need to find a way of passing in the currently logged in user to this function
             * var Course = _context.Class;//default?
             * if(user.type == "student"){
             * Course = //where course.courseId exists in students course list
             * }else{
             * Course = /where course.userID == user.id
             * }
             */
            //var id = Request.Query["Id"];
            var UserID = HttpContext.Session.GetInt32("UserID");
            var Course = from c in _context.Class select c;// gets the class table from the database **(still need to show only that specific teacher's courses)**
            if (UserID != null)
            {
                Course = Course.Where(c => c.UserId == UserID);
                var users = from u in _context.User select u;
                var user = users.Where(u => u.Id == UserID).ToList();
                ViewData["Student"] = user[0].UserType;
                return View(Course);
            }
            return Redirect("/Login/");//need a different default response
        }

        // The code below returns the value instead of the view() in users, I don't know how to get it any other way.
        //
        //[Route("Users/")]
        //[HttpGet]
        //public async Task<string> GetLogInId()
        //{
        //    return Request.Query["Id"];
        //}

        public string test()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            return UserID == null ? "null" : UserID.Value.ToString();
        }

        public IActionResult Create()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();
            ViewData["Student"] = user[0].UserType;
            return View(); // just brings the create page up
        }

        // Need a submit class to update the database
        [HttpPost]
        public async Task<IActionResult> Create(
            [Bind("ClassId,UserId,Department,CourseNumber,CourseName,NumOfCredits,Location,StartTime,EndTime")] Class cl,
            ICollection<string> day)
        {
            int? UserID = HttpContext.Session.GetInt32("UserID");
            await CreateMain(UserID ?? default(int), cl, day);
            _cache.Remove(CacheKeys.UserView);

            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();
            ViewData["Student"] = user[0].UserType;

            return Redirect("/Classes/");//return to index page after creating page
        }

        [HttpPost]
        public async Task<int> CreateMain(int UserID, Class c, ICollection<string> day)
        {
            c.DaysOfWeek = "";
            IEnumerator<string> e = day.GetEnumerator();//loop through string list thing and add to DaysOfWeek string
            for (int i = 0; i <= day.Count; i++)
            {
                if (e.Current != null)
                {
                    if (i > 1)
                    {
                        c.DaysOfWeek += " | ";
                    }
                    c.DaysOfWeek += (e.Current);
                }
                e.MoveNext();
            }
            c.UserId = UserID == null ? -1 : UserID;//"syntax sugar" c.UserId needs a guarentee that its not getting null
            _context.Class.Add(c);
            await _context.SaveChangesAsync();
            return 1;
        }
        public async Task<IActionResult> Edit(int? id)
        {


            if (id == null || _context.Class == null)
            {
                return NotFound();
            }

            var course = await _context.Class.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            int? UserID = HttpContext.Session.GetInt32("UserID");
            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();
            ViewData["Student"] = user[0].UserType;

            return View(course);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ClassId,UserId,Department,CourseNumber,CourseName,NumOfCredits,Location,DaysOfWeek,StartTime,EndTime")] Class c)
        {



            var oldclass = _context.Class.Where(x => x.ClassId == id).FirstOrDefault();

            //        try
            //          {
            //if(c.Department == null ||c.CourseNumber == null||c.CourseName== null||c.NumOfCredits == null||c.Location == null ||c.DaysOfWeek == null |c.StartTime == null||c.EndTime == null)
            //{
            // c.Department = oldclass.Department;
            // c.CourseNumber= oldclass.CourseNumber;
            // c.CourseName= oldclass.CourseName;
            // c.NumOfCredits= oldclass.NumOfCredits;
            // c.Location= oldclass.Location;
            // c.DaysOfWeek= oldclass.DaysOfWeek;
            // c.StartTime= oldclass.StartTime;
            // c.EndTime= oldclass.EndTime;

            //}

            _context.ChangeTracker.Clear();
            _context.Update(c);
            await _context.SaveChangesAsync();

            return Redirect("/Classes");
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Class == null)
            {
                return NotFound();
            }

            var course = await _context.Class
                .FirstOrDefaultAsync(m => m.ClassId == id);
            if (course == null)
            {
                return NotFound();
            }

            int? UserID = HttpContext.Session.GetInt32("UserID");
            var users = from u in _context.User select u;
            var user = users.Where(u => u.Id == UserID).ToList();
            ViewData["Student"] = user[0].UserType;

            return View(course);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,Class c)
        {
            var course = await _context.Class.FindAsync(id);
            _context.Class.Remove(c);
            await _context.SaveChangesAsync();
            _cache.Remove(CacheKeys.UserView);
            return RedirectToAction(nameof(Index));

            //if (_context.Class == null)
            //{
            //    return Problem("Entity set 'Assignment_1Context.User'  is null.");
            //}
            //var course = await _context.Class.FindAsync(id);

            //_context.Class.Remove(course);


            //await _context.SaveChangesAsync();
            //return Redirect("/Classes");
        }
    }
}

