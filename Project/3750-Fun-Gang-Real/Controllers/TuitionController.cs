using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Microsoft.EntityFrameworkCore;
using Assignment_1.Data;

namespace Assignment_1.Controllers
{
    public class TuitionController : Controller
    {
        private readonly Assignment_1Context _context; // declaration for the context object
        public TuitionController(Assignment_1Context context)
        {
            StripeConfiguration.ApiKey = "sk_test_51Mb6siLMv5EI8mC5NUvOtbB5GR8yROjRo4xmnxHzYifnxojwHC22T1u0y19TOS3PJxZ7ocCybKO2qXnyEkrV1Ytt00JF125KMq";
            _context = context; // makes it so we can get the database at any time
        }

        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession(int amount)
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                      UnitAmount = (amount == 0 ? 50 : amount * 100),
                      Currency = "usd",
                      ProductData = new SessionLineItemPriceDataProductDataOptions
                      {
                        Name = "Tuition_Payment",
                      },
                    },
                    Quantity = 1,
                  },
                },
                Mode = "payment",
                SuccessUrl = "https://localhost:7099/Tuition/Success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://localhost:7099/Tuition/Cancel",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        
        [HttpGet("/Tuition/Success")]
        public ActionResult OrderSuccess([FromQuery] string session_id)
        {
            //send back to DB
            //ViewData["amount"] = savedAmount;
            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);

            var customerService = new CustomerService();
            long? p = session.AmountSubtotal / 100;
            ViewData["amount"] = p;

            var UserID = HttpContext.Session.GetInt32("UserID");
            var user = from u in _context.User select u;
            if (UserID != null)
            {
                Assignment_1.Models.User a = user.Where(x => x.Id == UserID).First();

                //update user balance in database
                a.Balance += Convert.ToDecimal(p != null ? p : 0);
                _context.Update(a);
                _context.SaveChangesAsync();
                ViewData["Student"] = a.UserType;
            }
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
        public IActionResult Index()
        {
            var UserID = HttpContext.Session.GetInt32("UserID");
            var user = from u in _context.User select u;
            if (UserID != null)
            {
                Assignment_1.Models.User a = user.Where(x => x.Id == UserID).First();
                ViewData["Student"] = a.UserType;
                return View(a);
            }
            return Redirect("/Login/");
        }
    }
}
