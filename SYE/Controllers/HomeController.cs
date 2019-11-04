using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SYE.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using SYE.Helpers.Enums;

namespace SYE.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet, Route("GFC-Local-Start")]
        public IActionResult Index()
        {
            ViewBag.Title = "Give feedback on care - Care Quality Commission (CQC)";
            ViewBag.HideSiteTitle = true;
            if (TempData.ContainsKey("search"))
                TempData.Remove("search");            
            return View();
        }

        [EnableCors("GfcAllowedOrigins")]
        [Authorize(Policy = "ApiKeyPolicy")]
        [HttpPost, Route("website-redirect")]
        public IActionResult Index([FromBody] ProviderDetailsVM providerDetails)
        {
            ViewBag.Title = "Give feedback on care - Care Quality Commission (CQC)";
            ViewBag.HideSiteTitle = true;

            if (!string.IsNullOrEmpty(providerDetails.LocationId) && !string.IsNullOrEmpty(providerDetails.ProviderId) && !string.IsNullOrEmpty(providerDetails.LocationName) && providerDetails.CookieAccepted != null)
            {
                return RedirectToAction("SelectLocation", "Search", providerDetails);
            }
            else if (providerDetails.CookieAccepted != null)
            {
                return RedirectToAction("Index", "Search", new { CookieDisplay = providerDetails.CookieAccepted });
            }             
            else            
            {
                return GetCustomErrorCode(EnumStatusCode.CQCIntegrationPayLoadNullError, "Error with CQC PayLoad null on the redirection post request");                
            }            
           
        }       

        [EnableCors("GfcAllowedOrigins")]
        [Authorize(Policy = "ApiKeyPolicy")]
        [HttpGet, Route("website-redirect/{staticPage}/{cookieDisplay}")]
        public IActionResult Index(string staticPage, bool cookieAccepted)
        {
            ViewBag.Title = "Give feedback on care - Care Quality Commission (CQC)";
            ViewBag.HideSiteTitle = true;

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
