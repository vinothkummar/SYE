using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SYE.Models;
using SYE.Services;

namespace SYE.Controllers
{
    public class SearchController : Controller
    {
        private readonly IPageService _pageService;
        private readonly ISessionService _sessionService;

        public SearchController(IPageService pageService, ISessionService sessionService)
        {
            _pageService = pageService;
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index(string id)
        {
            ViewBag.ShowResults = true;
            return View();
        }


        [HttpPost]
        public IActionResult SelectLocation(SessionVM vm)
        {
            //Store the location we are giving feedback about
            _sessionService.SetSessionVars(vm);

            var value = HttpContext.Session.GetString("LocationName");


            //Set up our replacement text
            var replacements = new Dictionary<string, string>
            {
                {"!!location_name!!", vm.LocationName}
            };

            //Read the Schema and apply the replacements
            var userForm = _pageService.GetFormVm(replacements);

            //Store our schema in user session
            _sessionService.SaveFormVmToSession(userForm);
            
            return RedirectToAction("Index", "Form");
        }


    }
}