using Assignment_1.Data;
using Assignment_1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace Assignment_1.Controllers
{
	public class LoginController : Controller
	{
		private readonly Assignment_1Context _context;

		public LoginController(Assignment_1Context context)
		{

			_context = context;
		}
		public IActionResult Index()
		{
			var UserID = HttpContext.Session.GetInt32("UserID");
			if (UserID != null)
			{
                return Redirect("/Users/?Id=" + UserID);
            }
			return View();
		}

		public async Task<IActionResult> Validate(string Email, string Password)
		{
			//Debug.WriteLine("Email: " + Email);
			//Debug.WriteLine("Password: " + Password);
			if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
			{
                PasswordHasher<User> Hasher = new PasswordHasher<User>();

                var user = _context.User.Where(x => x.Email == Email).FirstOrDefault();

				
				if (user != null)
				{
                    ViewData["Student"] = user.UserType;
                    if (Hasher.VerifyHashedPassword(new User(), user.Password, Password) == PasswordVerificationResult.Success)
                    {
                        //creates a key (UserID) value (user.Id) pair in the browser session storage, can be retrieved as long as browser not closed
                        HttpContext.Session.SetInt32("UserID", user.Id);
                        return Redirect("/Users/?Id=" + user.Id);
                    }
                }
				//            if (user.UserType == "Student")
				//{
				//ViewData["isStudent"] = true;

				//}
				//else
				//{	
				//	ViewData["isStudent"] = false;

				//            }

				
            }
			
			return Redirect("/Login/");
		}
		
	}
}
