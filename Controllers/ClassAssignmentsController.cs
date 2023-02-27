using Assignment_1.Data;
using Assignment_1.Migrations;
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
            return View();
        }
    }
}
