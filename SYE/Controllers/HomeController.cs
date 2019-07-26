using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Globalization;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SYE.Models;

namespace SYE.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            ViewBag.HideSiteTitle = true;
            return View();
        }


        [Route("set-version")]
        public IActionResult SetVersion(string v = "")
        {
            //Set the version for A/B testing
            //This will be used when we load the form
            ViewBag.HideSiteTitle = true;
            HttpContext.Session.SetString("FormVersion", v);
            return View("Index");
        }

        // Error return pages are can be configured here. 
        // I have added basics here. Add and adapt as appropriate
        // once tickets are raised with specific requirements.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("Error/{statusCode?}")]
        public IActionResult Error(int? statusCode = null)
        {
            //ViewBag.ShowBackButton = false;
            var statusCodeReExecuteFeature = _httpContextAccessor?.HttpContext?.Features?.Get<IStatusCodeReExecuteFeature>();

            //Following will not be populated on 404
            var exceptionHandlerPathFeature = _httpContextAccessor?.HttpContext?.Features?.Get<IExceptionHandlerPathFeature>();

            // 400, 500 and default are coded if you need to add more error codes, add them here
            // Please change error messages and if needed View for error page.
            // Alternatively you can also add specific error views like 400.cshtml and "return view("400");"
            switch (statusCode)
            {
                case 404:
                    _logger.LogError("page not found");
                    ViewBag.ErrorTitle = "Page not found";
                    ViewBag.ErrorMessage = "If you typed the web address, check it is correct. If you pasted the web address, check you copied the entire address.";
                    break;
                case 500:
                case 503:
                    _logger.LogError("Sorry, there is a problem with this form");
                    ViewBag.ErrorTitle = "Sorry, there is a problem with this form";
                    ViewBag.ErrorMessage = "Try clicking your browser's back button or try again later.";
                    break;
                default:
                    ViewBag.ErrorTitle = "The service is unavailable";
                    ViewBag.ErrorMessage = "Try clicking your browser's back button or try again later.";
                    break;
            }
            // Ideally default error view "Error.cshtml" should be changed and model removed 
            // as we do not require Trace/Activity Id there
            return View(
                new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                }
            );
        }

        [Route("Clear-Data")]
        public IActionResult ClearData()
        {
            ControllerContext.HttpContext.Session.Clear();
            return new RedirectResult("/");
        }
    }
}
