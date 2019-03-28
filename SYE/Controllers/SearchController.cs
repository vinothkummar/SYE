using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SYE.Models;
using SYE.Services;
using SYE.ViewModels;
using SYE.Models;
using SYE.Services;

namespace SYE.Controllers
{
    [Route("[controller]")]
    public class SearchController : Controller
    {
        private readonly int _pageSize = 10;
        private readonly ISearchService _searchService;
        private readonly IFormService _formService;
        private readonly ISessionService _sessionService;

        public SearchController(ISearchService searchService, IFormService formService, ISessionService sessionService)
        {
            _searchService = searchService;
            _formService = formService;
            _sessionService = sessionService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index([FromQuery] string search, [FromQuery] int pageNo)
        {
            try
            {
                var viewModel = GetViewModel(search, pageNo);

                ViewBag.ShowResults = true;
                return View(viewModel);
            }
            catch (Exception e)
            {
                //log error
                return StatusCode(500);
            }
        }
        [HttpGet]
        [Route("{search},{pageNo}")]
        public IActionResult GetPaginateResult(string search, int pageNo)
        {
            try
            {
                var viewModel = GetViewModel(search, pageNo);
                return View("Index", viewModel);

            }
            catch (Exception e)
            {
                //log error
                return StatusCode(500);
            }
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
        /// <summary>
        /// loads up the view model with paged data when there is a search string and page number
        /// otherwise it just returns a new view model
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        private SearchResultsViewModel GetViewModel(string search, int pageNo)
        {
            var returnViewModel = new SearchResultsViewModel();

            if (!string.IsNullOrEmpty(search) && pageNo > 0)
            {

                var results = _searchService.GetPaginatedResult(search, pageNo, _pageSize).Result;
                returnViewModel.ShowResults = true;
                returnViewModel.Search = search;
                returnViewModel.Data = results;
                returnViewModel.PageSize = _pageSize;
                returnViewModel.Count = _searchService.GetCount();
                returnViewModel.CurrentPage = pageNo;
            }
            return returnViewModel;
        }
    }
}