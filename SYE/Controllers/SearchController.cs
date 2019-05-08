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
        private readonly int _maxSearchChars = 1000;
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

        [HttpPost]//applies the filter & does a search
        public IActionResult SearchResults(string search, List<SelectItem> facets = null)
        {
            var selectedFacets = string.Empty;
            if (facets != null)
            {
                selectedFacets = string.Join(',', facets.Where(x => x.Selected).Select(x => x.Text).ToList());
            }
                
            return GetSearchResult(search, 1, selectedFacets);
        }

        [HttpGet]//searches
        public IActionResult SearchResults(string search, int pageNo = 1, string selectedFacets = "")
        {
            return GetSearchResult(search, pageNo, selectedFacets);
        }

        [HttpGet]
        public IActionResult LocationNotFound()
        {
            var defaultServiceName = "the service";
            
            try
            {
                //Store the user entered details
                _sessionService.SetUserSessionVars(new UserSessionVM{LocationId = "0", LocationName = defaultServiceName, ProviderId = ""});

                //Set up our replacement text
                var replacements = new Dictionary<string, string>
                {
                    {"!!location_name!!", defaultServiceName}
                };

                //Load the Form into Session
                _sessionService.LoadLatestFormIntoSession(replacements);

                return RedirectToAction("Index", "Form");
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
                return StatusCode(500);
            }
        }

        private IActionResult GetSearchResult(string search, int pageNo, string selectedFacets)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(search))
                {
                    //reset search
                    return RedirectToAction("Index", new { isError = true });
                }

                if (search.Length > _maxSearchChars)
                {
                    return View(new SearchResultsVM
                    {
                        Search = search,
                        ShowExceededMaxLengthMessage = true, Facets = new List<SelectItem>(),
                        Data = new List<SearchResult>()
                    });
                }

                var newSearch = SetNewSearch(search);

                var viewModel = GetViewModel(search, pageNo, selectedFacets, newSearch);

                return View(viewModel);
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
                returnViewModel.Facets = SubmissionHelper.ConvertList(_searchService.GetFacets());
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