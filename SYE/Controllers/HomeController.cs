using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SYE.Models;

namespace SYE.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string v = "")
        {
            //Set the version for A/B testing
            //This will be used when we load the form
            HttpContext.Session.SetString("FormVersion", v);
            ViewBag.ShowBackButton = false;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewBag.ShowBackButton = false;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
