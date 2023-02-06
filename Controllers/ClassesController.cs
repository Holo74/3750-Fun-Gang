using Assignment_1.Data;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_1.Controllers
{
    public class ClassesController : Controller
    {
        private readonly Assignment_1Context _context; // declaration for the context object

        public ClassesController(Assignment_1Context context)
        {
            _context = context; // makes it so we can get the database at any time
        }
        
        public IActionResult Index()
        {
            var Course = _context.Class; // gets the class table from the database **(still need to show only that specific teacher's courses)**
            return View(Course); //outputs the data
        }

        public IActionResult Create()
        {
            return View(); // just brings the create page up
        }

        // Need a submit class to update the database
    }
}
