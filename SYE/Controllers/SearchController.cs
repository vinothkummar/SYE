using Microsoft.AspNetCore.Mvc;
using SYE.Models;
using SYE.Services;
using SYE.ViewModels;

namespace SYE.Controllers
{
    [Route("[controller]")]
    public class SearchController : Controller
    {
        private readonly int _pageSize = 10;
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index([FromQuery] string search, [FromQuery] int pageNo)
        {
            var viewModel = GetViewModel(search, pageNo);

            ViewBag.ShowResults = true;
            return View(viewModel);

        }
        [HttpGet]
        [Route("{search},{pageNo}")]
        public IActionResult GetPaginateResult(string search, int pageNo)
        {
            var viewModel = GetViewModel(search, pageNo);

            ViewBag.ShowResults = true;
            return View("Index", viewModel);
        }

        [HttpPost]
        public IActionResult Index(string search)
        {
            var viewModel = GetViewModel(search, 1);

            ViewBag.ShowResults = true;
            return View(viewModel);
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