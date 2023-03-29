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
    }
}
