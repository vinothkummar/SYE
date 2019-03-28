using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SYE.Models;
using SYE.Services;
using SYE.ViewModels;
using SYE.Helpers;

namespace SYE.Controllers
{
    public class SearchController : Controller
    {
        private readonly int _pageSize = 20;
        private readonly ISearchService _searchService;
        private readonly ISessionService _sessionService;

        public SearchController(ISearchService searchService, ISessionService sessionService)
        {
            _searchService = searchService;
            _sessionService = sessionService;
        }


        [HttpGet]
        public IActionResult Index(string search, int pageNo = 1)
        {
            try
            {
                var viewModel = GetViewModel(search, pageNo);

                ViewBag.ShowResults = true;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500);
            }
        }


        [HttpGet]
        public IActionResult GetPaginateResult(string search, int pageNo)
        {
            try
            {
                var viewModel = GetViewModel(search, pageNo);
                return View("Index", viewModel);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500);
            }
        }


        [HttpPost]
        public IActionResult SelectLocation(UserSessionVM vm)
        {
            try
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
            catch(Exception ex)
            {
                //log error
                return NotFound();
            }
        }

        [HttpPost]
        public IActionResult SelectFacets(IEnumerable<SelectListItem> facets)
        {
            try
            {
                var viewModel = GetViewModel("", 0);
                return View("Index", viewModel);

            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500);
            }
        }


        /// <summary>
        /// loads up the view model with paged data when there is a search string and page number
        /// otherwise it just returns a new view model
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        private SearchResultsVM GetViewModel(string search, int pageNo)
        {
            var returnViewModel = new SearchResultsVM();

            if (!string.IsNullOrEmpty(search) && pageNo > 0)
            {

                var results = _searchService.GetPaginatedResult(search, pageNo, _pageSize).Result;
                returnViewModel.ShowResults = true;
                returnViewModel.Search = search;
                returnViewModel.Data = results;
                returnViewModel.PageSize = _pageSize;
                returnViewModel.Count = _searchService.GetCount();
                returnViewModel.Facets = ViewHelper.ConvertList(_searchService.GetFacets());
                returnViewModel.TypeOfService = _searchService.GetFacets();
                returnViewModel.CurrentPage = pageNo;
            }
            return returnViewModel;
        }
    }
}