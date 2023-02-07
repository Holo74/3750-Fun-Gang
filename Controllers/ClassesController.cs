using Assignment_1.Data;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace Assignment_1.Controllers
{
    public class ClassesController : Controller
    {
        private readonly Assignment_1Context _context; // declaration for the context object

        public ClassesController(Assignment_1Context context)
        {
            _context = context; // makes it so we can get the database at any time
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
            var id = Request.Query["Id"];
            var Course = _context.Class;// gets the class table from the database **(still need to show only that specific teacher's courses)**
            return View(Course);
        }

        // The code below returns the value instead of the view() in users, I don't know how to get it any other way.
        //
        //[Route("Users/")]
        //[HttpGet]
        //public async Task<string> GetLogInId()
        //{
        //    return Request.Query["Id"];
        //}

        public IActionResult Create()
        {
            return View(); // just brings the create page up
        }

        // Need a submit class to update the database
    }
}
