using Microsoft.AspNetCore.Mvc;
using SYE.Models;
using SYE.Services;

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
            return View();
        }


        [HttpPost]
        public IActionResult Index(string search)
        {
            var results = _searchService.GetServiceProviders(search);
            var viewModel = new SearchResultsViewModel {SearchResults = results};

            ViewBag.ShowResults = true;
            return View(viewModel);
        }

    }
}