using Microsoft.AspNetCore.Mvc;
using StripeView.Models;
using System.Diagnostics;

namespace StripeView.Controllers
{
    public class HomeController : Controller
    {
    
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}
