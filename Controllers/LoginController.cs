using Assignment_1.Data;
using Assignment_1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
			//Debug.WriteLine("AAAAAAAAAAAAAAAAAAAAAA");
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
