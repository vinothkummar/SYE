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
using SYE.Helpers.Enums;

namespace SYE.Controllers
{
    public class SearchController : BaseController
    {
        private readonly int _pageSize = 20;
        private readonly int _maxSearchChars = 1000;
        private readonly int _minSearchChars = 1;
        private readonly ISearchService _searchService;
        private readonly ISessionService _sessionService;
        private readonly IOptions<ApplicationSettings> _config;
        private readonly IGdsValidation _gdsValidate;

        public SearchController(ISearchService searchService, ISessionService sessionService, IOptions<ApplicationSettings> config, IGdsValidation gdsValidate)
        {
            _searchService = searchService;
            _sessionService = sessionService;
            _config = config;
            _gdsValidate = gdsValidate;
        }

        [HttpGet("search/find-a-service")]
        public IActionResult Index(string errorMessage, string search, bool cookieDisplay)
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
                ex.Data.Add("GFCError", "Unexpected error loading search page.");
                throw ex;
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
                _sessionService.ClearNavOrder();
                //Set up our replacement text
                var replacements = new Dictionary<string, string>
                {
                    {"!!location_name!!", defaultServiceName}
                };

                try
                {
                    //Load the Form into Session
                    _sessionService.LoadLatestFormIntoSession(replacements);
                }
                catch
                {
                    return GetCustomErrorCode(EnumStatusCode.SearchLocationNotFoundJsonError, "Error in location not found. json form not loaded");
                }

                var serviceNotFoundPage = _config.Value.ServiceNotFoundPage;
                return RedirectToAction("Index", "Form", new { id = serviceNotFoundPage });
            }
            catch (Exception ex)
            {
                ex.Data.Add("GFCError", "Unexpected error in location not found.");
                throw ex;
            }
        }

        //[HttpPost]
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
                try
                {
                    //Load the Form and the search url into Session
                    _sessionService.LoadLatestFormIntoSession(replacements);
                    var searchUrl = Request.Headers["Referer"].ToString();
                    _sessionService.SaveSearchUrl(searchUrl);

                    var startPage = _config.Value.FormStartPage;
                    return RedirectToAction("Index", "Form", new { id = startPage });
                }
                catch
                {
                    return GetCustomErrorCode(EnumStatusCode.SearchSelectLocationJsonError, "Error selecting location. json form not loaded");
                }
            }
            catch (Exception ex)
            {
                ex.Data.Add("GFCError", "Unexpected error selecting location: '" + vm.LocationName + "'");
                throw ex;
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
                if (! (cleanSearch.Length <= _maxSearchChars && cleanSearch.Length >= _minSearchChars))
                {
                    errorMessage = $"Your search must be 1,000 characters or less";
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
                if (viewModel == null)
                {
                    return GetCustomErrorCode(EnumStatusCode.SearchUnavailableError,
                        "Search unavailable: Search string='" + search + "'");
                }

                ViewBag.BackLink = new BackLinkVM { Show = true, Url = Url.Action("Index", "Home"), Text = "Back" };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ex.Data.Add("GFCError", "Unexpected error in search :'" + search + "'");
                throw ex;
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
                SearchServiceResult searchResult = null;
                try
                {
                    searchResult = _searchService.GetPaginatedResult(search, pageNo, _pageSize, refinementFacets, newSearch).Result;
                }
                catch
                {
                    return null;//search is not working for some reason
                }                
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
    }
}