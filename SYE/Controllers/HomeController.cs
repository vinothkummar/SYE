using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SYE.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

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
            ViewBag.Title = "Give feedback on care - Care Quality Commission (CQC)";
            ViewBag.HideSiteTitle = true;
            var emptyModel = new ProviderDetailsVM();
            return View(emptyModel);
        }

        //[Route("Index/{locationId}/{providerId}/{locationName}")]
        //public IActionResult Index(string locationId, string providerId, string locationName)
        //{
        //    ViewBag.Title = "Give feedback on care - Care Quality Commission (CQC)";
        //    ViewBag.HideSiteTitle = true;
        //    var providerDetails = new ProviderDetailsVM() { LocationId = locationId, ProviderId = providerId, LocationName = locationName };
        //    return View(providerDetails);
        //}

        [Authorize(Policy = "ApiKeyPolicy")]        
        [HttpPost, Route("website-redirect")]
        public IActionResult Index([FromBody] ProviderDetailsVM providerDetails)
        {
            ViewBag.Title = "Give feedback on care - Care Quality Commission (CQC)";
            ViewBag.HideSiteTitle = true;
            if (providerDetails != null)
            {
                return RedirectToAction("SelectLocation", "Search", providerDetails);
            }
            else
            {
                return RedirectToAction("Index", "Search");
            }
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
