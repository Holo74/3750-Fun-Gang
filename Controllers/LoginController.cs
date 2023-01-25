using Assignment_1.Data;
using Assignment_1.Models;
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
			Debug.WriteLine("Email: " + Email);
			Debug.WriteLine("Password: " + Password);
			if (!string.IsNullOrEmpty(Email) && !string.IsNullOrEmpty(Password))
			{
				
				var user = _context.User.Where(x => x.Email == Email).FirstOrDefault();

				if (user != null)
				{
					return Redirect("/Users/Details/" + user.Id);
				}
			}
			
			return Redirect("/Login/");
		}
		
	}
}
