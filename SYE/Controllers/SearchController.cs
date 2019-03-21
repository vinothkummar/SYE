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
            if (string.IsNullOrEmpty(search))
            {
                //called from initial page load
                return View(new SearchResultsViewModel {Search = string.Empty});
            }
            //called from paging
            var viewModel = GetViewModel(search, pageNo);

            ViewBag.ShowResults = true;
            return View("Index", viewModel);
        }
        //[HttpGet]
        //[Route("{search},{pageNo}")]
        //public IActionResult GetPaginateResult(string search, int pageNo)
        //{
        //    var viewModel = GetViewModel(search, pageNo);

        //    ViewBag.ShowResults = true;
        //    return View("Index", viewModel);
        //}

        [HttpPost]
        public IActionResult Index(string search)
        {
            var viewModel = GetViewModel(search, 1);

            ViewBag.ShowResults = true;
            return View(viewModel);
        }
        /// <summary>
        /// loads up the view model with paged data
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        private SearchResultsViewModel GetViewModel(string search, int pageNo)
        {
            var results = _searchService.GetPaginatedResult(search, pageNo, _pageSize).Result;

            var viewModel = new SearchResultsViewModel()
            {
                Search = search, Data = results, PageSize = _pageSize, Count = _searchService.GetCount(),
                CurrentPage = pageNo
            };
            return viewModel;
        }
    }
}