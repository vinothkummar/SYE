using System;
using System.Collections.Generic;
using System.Linq;
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
        public IActionResult Index(bool isError)
        {
            try
            {
                return View(new SearchResultsVM{ShowIncompletedSearchMessage = isError});
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500);
            }
        }

        [HttpGet]
        public IActionResult SearchResults(string search, int pageNo = 1, List<SelectListItem> facets = null, string selectedFacets="")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(search))
                {
                    //reset search
                    return RedirectToAction("Index", new {isError = true});
                }
                var newSearch = SetNewSearch(search);

                var refinementFacets = string.Empty;
                if (!string.IsNullOrEmpty(selectedFacets))
                {
                    refinementFacets = selectedFacets;
                }
                else if (facets != null && facets.Count > 1)
                {
                    refinementFacets = string.Join(',', facets.Where(x => x.Selected).Select(x => x.Text).ToList());                    
                }

                var viewModel = GetViewModel(search, pageNo, refinementFacets, newSearch);

                return View(viewModel);
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

        [HttpGet]
        public IActionResult SelectFacets(string search, List<SelectListItem> facets)
        {
            try
            {
                var refinementFacets = string.Empty;
                if (facets != null && facets.Count > 1)
                {
                    refinementFacets = string.Join(',', facets.Where(x => x.Selected).Select(x => x.Text).ToList());
                }
                var viewModel = GetViewModel(search, 1, refinementFacets, false);
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
        /// otherwise it just returns a new view model with a show error flag
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageNo"></param>
        /// <param name="refinementFacets">comma separated list of selected facets to filter on</param>
        /// <returns></returns>
        private SearchResultsVM GetViewModel(string search, int pageNo, string refinementFacets, bool newSearch)
        {
            var returnViewModel = new SearchResultsVM();

            if (!string.IsNullOrEmpty(search) && pageNo > 0)
            {
                returnViewModel.Data = _searchService.GetPaginatedResult(search, pageNo, _pageSize, refinementFacets, newSearch).Result;
                returnViewModel.ShowResults = true;
                returnViewModel.Search = search;                
                returnViewModel.PageSize = _pageSize;
                returnViewModel.Count = _searchService.GetCount();
                returnViewModel.Facets = ViewHelper.ConvertList(_searchService.GetFacets());
                returnViewModel.TypeOfService = _searchService.GetFacets();
                returnViewModel.CurrentPage = pageNo;

                if (returnViewModel.Facets != null && (! string.IsNullOrEmpty(refinementFacets)) && ! newSearch)
                {
                    foreach (var facet in returnViewModel.Facets)
                    {
                        facet.Selected = (refinementFacets.Contains(facet.Text));
                    }
                }
            }

            return returnViewModel;
        }
        /// <summary>
        /// saves the search and checks saved search to see if it is a new search       
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        private bool SetNewSearch(string search)
        {
            bool newSearch = true;

            if (!string.IsNullOrEmpty(search))
            {
                var previousSearch = _sessionService.GetUserSearch();
                newSearch = !(search.Equals(previousSearch, StringComparison.CurrentCultureIgnoreCase));
                _sessionService.SaveUserSearch(search);
            }

            return newSearch;
        }
        private string GetPreviousSearch()
        {
            var previousSearch = _sessionService.GetUserSearch();
            return previousSearch;
        }
    }
}