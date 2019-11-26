using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SYE.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using SYE.Helpers.Enums;
using SYE.Services;
using Microsoft.AspNetCore.Http.Internal;
using System.IO;
using System.Text;
using System;
using System.Linq;
using Microsoft.Extensions.Options;
using SYE.Helpers.Algorithm;

namespace SYE.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<ApplicationSettings> _config;
        public ISessionService _sessionService { get; }

        private readonly ILocationService _locationService;

        public HomeController(ILogger<HomeController> logger, 
            IHttpContextAccessor httpContextAccessor, ISessionService sessionService, ILocationService locationService,
            IOptions<ApplicationSettings> config)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _sessionService = sessionService;
            _locationService = locationService;
            _config = config;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var redirectUrl = _config.Value.RedirectUrl;
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                Response.Redirect(redirectUrl);
            }
            
            
            ViewBag.Title = "Give feedback on care - Care Quality Commission (CQC)";
            ViewBag.HideSiteTitle = true;
            if (TempData.ContainsKey("search"))
                TempData.Remove("search");            
            return View();
        }
       
        [Authorize(Policy = "ApiKeyPolicy")]
        [HttpPost, Route("website-redirect")]
        public IActionResult Index([FromForm] ProviderDetailsVM providerDetails)
        {          
            ViewBag.Title = "Give feedback on care - Care Quality Commission (CQC)";

            ViewBag.HideSiteTitle = true;

            if (!string.IsNullOrEmpty(providerDetails.LocationId) && !string.IsNullOrEmpty(providerDetails.ProviderId) && !string.IsNullOrEmpty(providerDetails.LocationName) && !string.IsNullOrEmpty(providerDetails.CookieAccepted))
            {
                _sessionService.SetCookieFlagOnSession(providerDetails.CookieAccepted.ToLower().Trim());

                var result = _locationService.GetByIdAsync(providerDetails.LocationId).Result;

                if (result == null)
                {
                    _logger.LogError("Error with CQC PayLoad; Provider Information not exist in the system", EnumStatusCode.CQCIntegrationPayLoadNullError);
                   
                    return RedirectToAction("Index", "Search");
                }     
                
                return RedirectToAction("SelectLocation", "Search", routeValues: providerDetails);               
            }
            else if (!string.IsNullOrEmpty(providerDetails.CookieAccepted))
            {
                _sessionService.SetCookieFlagOnSession(providerDetails.CookieAccepted.ToLower().Trim());
                return RedirectToAction("Index", "Search");
            }             
            else            
            {
                return GetCustomErrorCode(EnumStatusCode.CQCIntegrationPayLoadNullError, "Error with CQC PayLoad null on the redirection post request"); 
            }            
           
        }       
             
        [HttpGet, Route("website-redirect/{staticPage}/{cookieAccepted}")]
        public IActionResult Index(string staticPage, string cookieAccepted)
        {           

            ViewBag.Title = "Give feedback on care - Care Quality Commission (CQC)";
            ViewBag.HideSiteTitle = true;

            if (!string.IsNullOrEmpty(cookieAccepted))
            {
                _sessionService.SetCookieFlagOnSession(cookieAccepted.ToLower().Trim());
            }
            else
            {
                return GetCustomErrorCode(EnumStatusCode.CQCIntegrationPayLoadNullError, "Error with CQC Cookiee PayLoad redirection");
            }
           

            switch (staticPage)
            {
                case "how-we-handle-information":
                    return RedirectToAction("Index", "HowWeUseYourInformation");
                case "accessibility":
                    return RedirectToAction("Index", "Accessibility");
                case "report-a-problem":
                    return RedirectToAction("Feedback", "Help");
                default:
                    break;
            }
            return RedirectToAction("Index", "Home");
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

        [Route("Clear-Data")]
        public IActionResult ClearData()
        {
            ControllerContext.HttpContext.Session.Clear();
            return new RedirectResult("/");
        }   
    }
}
