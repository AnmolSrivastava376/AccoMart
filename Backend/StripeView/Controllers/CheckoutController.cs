using Microsoft.AspNetCore.Mvc;
using StripeView.Models;
using System.Diagnostics;

namespace StripeView.Controllers
{
    public class Checkout : Controller
    {
        public IActionResult OrderConfirmation()
        {
            return View();
        }

    }
}
