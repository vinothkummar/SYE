using System;
using System.Collections.Generic;
using System.Linq;
using GDSHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SYE.Models;
using SYE.Services;
using SYE.ViewModels;
using SYE.Helpers;
using Microsoft.Extensions.Logging;

namespace SYE.Controllers
{
    public class SearchController : Controller
    {
        private readonly int _pageSize = 20;
        private readonly int _maxSearchChars = 100;
        private readonly int _minSearchChars = 2;
        private readonly ISearchService _searchService;
        private readonly ISessionService _sessionService;
        private readonly IOptions<ApplicationSettings> _config;
        private readonly ILogger _logger;
        private readonly IGdsValidation _gdsValidate;

        public SearchController(ISearchService searchService, ISessionService sessionService, IOptions<ApplicationSettings> config, ILogger<SearchController> logger, IGdsValidation gdsValidate)
        {
            _searchService = searchService;
            _sessionService = sessionService;
            _config = config;
            _logger = logger;
            _gdsValidate = gdsValidate;
        }

        [HttpGet("search/find-a-service")]
        public IActionResult Index(string errorMessage, string search)
        {
            ViewBag.BackLink = new BackLinkVM { Show = true, Url = Url.Action("Index", "Home"), Text = "Back" };

            //Make Sure we have a clean session
            _sessionService.ClearSession();

            try
            {
                ViewBag.Title = "Find a service - Give feedback on care";
                return View(new SearchResultsVM { ErrorMessage = errorMessage, ShowIncompletedSearchMessage = (errorMessage != null), Search = search});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading search page.");
                return StatusCode(500);
            }
        }


    
        private static readonly HashSet<char> allowedChars = new HashSet<char>(@"1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz.,'()?!#$£%^@*;:+=_-/ ");
        private static readonly List<string> restrictedWords = new List<string> { "javascript", "onblur", "onchange", "onfocus", "onfocusin", "onfocusout", "oninput", "onmouseenter", "onmouseleave",
            "onselect", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onscroll", "ontouchstart",
            "ontouchend", "ontouchmove", "ontouchcancel", "onwheel" };

        [HttpGet, Route("/search/results")]//searches
        public IActionResult SearchResults(string search, int pageNo = 1, string selectedFacets = "")
        {
            var cleanSearch = _gdsValidate.CleanText(search, true, restrictedWords, allowedChars);

            var errorMessage = ValidateSearch(cleanSearch);
            if (errorMessage != null)
            {
                return RedirectToAction("Index", new {errorMessage, search = cleanSearch});
            }

            ViewBag.Title = "Results for " + cleanSearch + " - Give feedback on care";

            return GetSearchResult(cleanSearch, pageNo, selectedFacets);
        }


        [HttpPost, Route("/search/results")]//applies the filter & does a search
        public IActionResult SearchResults(string search, List<SelectItem> facets = null)
        {
            var cleanSearch = _gdsValidate.CleanText(search, true, restrictedWords, allowedChars);
            if (string.IsNullOrEmpty(cleanSearch)) return RedirectToAction("Index", new { isError = "true"});

            var selectedFacets = string.Empty;
            if (facets != null)
            {
                selectedFacets = string.Join(',', facets.Where(x => x.Selected).Select(x => x.Text).ToList());
            }

            ViewBag.Title = "Results for " + cleanSearch + " - Give feedback on care";

            return GetSearchResult(cleanSearch, 1, selectedFacets);
        }

        
        [HttpGet]
        public IActionResult LocationNotFound()
        {
            var defaultServiceName = "the service";

            try
            {
                //Store the user entered details
                _sessionService.SetUserSessionVars(new UserSessionVM { LocationId = "0", LocationName = defaultServiceName, ProviderId = "" });

                //Set up our replacement text
                var replacements = new Dictionary<string, string>
                {
                    {"!!location_name!!", defaultServiceName}
                };

                //Load the Form into Session
                _sessionService.LoadLatestFormIntoSession(replacements);

                var serviceNotFoundPage = _config.Value.ServiceNotFoundPage;
                return RedirectToAction("Index", "Form", new { id = serviceNotFoundPage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading location not found page.");
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

                var startPage = _config.Value.FormStartPage;
                return RedirectToAction("Index", "Form", new { id = startPage });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error selecting location.");
                return StatusCode(500);
            }
        }

        private string ValidateSearch(string cleanSearch)
        {
            string errorMessage = null;
            if (string.IsNullOrEmpty(cleanSearch))
            {
                errorMessage = "Enter the name of a service, its address, postcode or a combination of these";
            }
            else
            {
                if (! (cleanSearch.Length < _maxSearchChars && cleanSearch.Length > _minSearchChars))
                {
                    errorMessage = string.Format("Enter between {0} and {1} characters", _minSearchChars, _maxSearchChars);
                }
            }

            return errorMessage;
        }
        private IActionResult GetSearchResult(string search, int pageNo, string selectedFacets)
        {
            //This is commented out as it is causing Facets to not work
            //Make Sure we have a clean session
            //_sessionService.ClearSession();

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
                        ShowExceededMaxLengthMessage = true,
                        Facets = new List<SelectItem>(),
                        Data = new List<SearchResult>()
                    });
                }

                var newSearch = SetNewSearch(search);

                var viewModel = GetViewModel(search, pageNo, selectedFacets, newSearch);

                ViewBag.BackLink = new BackLinkVM { Show = true, Url = Url.Action("Index", "Home"), Text = "Back" };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting search results.");
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
                var searchResult = _searchService.GetPaginatedResult(search, pageNo, _pageSize, refinementFacets, newSearch).Result;
                returnViewModel.Data = searchResult?.Data?.ToList() ?? new List<SearchResult>();
                returnViewModel.ShowResults = true;
                returnViewModel.Search = search;
                returnViewModel.PageSize = _pageSize;
                returnViewModel.Count = searchResult?.Count ?? 0;
                returnViewModel.Facets = SubmissionHelper.ConvertList(searchResult?.Facets);
                returnViewModel.TypeOfService = searchResult?.Facets;
                returnViewModel.CurrentPage = pageNo;

                if (returnViewModel.Facets != null && (!string.IsNullOrEmpty(refinementFacets)) && !newSearch)
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