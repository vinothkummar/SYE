using Microsoft.AspNetCore.Mvc;
using SYE.Models;
using SYE.Services;
using SYE.ViewModels;

namespace SYE.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View(new SearchResultsViewModel{Search = string.Empty});
        }


        [HttpPost]
        public IActionResult Index(string search)
        {
            var results = _searchService.GetServiceProviders(search);
            var viewModel = new SearchResultsViewModel {Search = search, SearchResults = results};

            ViewBag.ShowResults = true;
            return View(viewModel);
        }

    }
}