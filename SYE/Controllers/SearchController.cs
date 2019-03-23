using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SYE.Models;
using SYE.Services;

namespace SYE.Controllers
{
    public class SearchController : Controller
    {
        private readonly IFormService _formService;
        private readonly ISessionService _sessionService;

        public SearchController(IFormService formService, ISessionService sessionService)
        {
            _formService = formService;
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index(string query)
        {
            if (!string.IsNullOrEmpty(query))
            {
                ViewBag.ShowResults = true;
            }
            
            return View();
        }


        [HttpPost]
        public IActionResult SelectLocation(UserSessionVM vm)
        {
            //Store the location we are giving feedback about
            _sessionService.SetUserSessionVars(vm);
            
            //Set up our replacement text
            var replacements = new Dictionary<string, string>
            {
                {"!!location_name!!", vm.LocationName}
            };

            //Load the Form into Session
            _sessionService.LoadLatestFormIntoSession(replacements);
           
            return RedirectToAction("Index", "Form");
        }
        
    }
}