using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Assignment_1.Controllers
{
    public class TuitionController : Controller
    {
        public TuitionController()
        {
            StripeConfiguration.ApiKey = "sk_test_4eC39HqLyjWDarjtT1zdp7dc";
        }
        int savedAmount = 0;//hackey workaround for getting the amount passed into the DB

        [HttpPost("create-checkout-session")]
        public ActionResult CreateCheckoutSession(int amount)
        {
            savedAmount = amount * 100;
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
                SuccessUrl = "https://localhost:7099/Tuition/Success/",
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
            long? p = session.AmountSubtotal;
            ViewData["amount"] = p;

            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
