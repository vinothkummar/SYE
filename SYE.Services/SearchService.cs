using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using SYE.Models;
using SYE.Repository;
using SYE.Services.Helpers;
using SYE.Services.Wrappers;
using SearchResult = SYE.Models.SearchResult;

namespace SYE.Services
{
    public interface ISearchService
    {
        Task<List<SearchResult>> GetPaginatedResult(string search, int currentPage, int pageSize = 10);
        long GetCount();
    }
    public class SearchService : ISearchService
    {
        //private static SearchServiceClient _searchClient;
        private static ICustomSearchIndexClient _indexClientWrapper;

        private static long _count;

        public SearchService()//TODO remove this (amend startup.cs)
        {

        }
        public SearchService(ICustomSearchIndexClient indexClientWrapper)
        {
            _indexClientWrapper = indexClientWrapper;
        }    

        public async Task<List<SearchResult>> GetPaginatedResult(string search, int currentPage, int pageSize)
        {
            var data = await GetDataAsync(search, currentPage, pageSize);
            return data;
        }

        public long GetCount()
        {
            return _count;
        }

        #region Private Methods
        private async Task<List<SearchResult>> GetDataAsync(string search, int currentPage, int pageSize)
        {
            var sp = new SearchParameters
            {
                IncludeTotalResultCount = true, Skip = ((currentPage - 1) * pageSize), Top = pageSize
            };

            var searchResult = await _indexClientWrapper.SearchAsync(search, sp);

            if (searchResult.Count.HasValue)
            {
                _count = (long )searchResult.Count;
            }
            
            var results = searchResult.Results.Select(m => m.Document).ToList();
     
            return SearchHelper.ConvertResults(results);
        }

        #endregion

        #region commented out code probably use later
        /*         
using System.Collections.Generic;
using System.Linq;
using LocationImporterApi.Models;
using Microsoft.AspNetCore.Mvc;
using POC.Model;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;

namespace SYE_POC.Controllers
{
    public class AzureSearchController : Controller
    {
        private static SearchServiceClient _searchClient;
        private static ISearchIndexClient _indexClientWrapper;
        private static string _indexName = "sye-poc-index";

        public static string errorMessage;

        private void InitSearch()
        {
            var searchServiceName = "sye-poc-azure-search";
            var apiKey = "636F112CE66183702BCB07925E6EBB59";

            _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            _indexClientWrapper = _searchClient.Indexes.GetClient(_indexName);
        }


        [HttpGet]
        public IActionResult Index()
        {
            var vm = new SearchResultVM
            {
                CurrentPage = 1
            };
            return View(vm);
        }


        [HttpPost]
        public IActionResult Index(SearchResultVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);
            
            var sp = new SearchParameters();

            var results = _indexClientWrapper.Documents.Search(vm.SearchTerm, sp).Results.Select(m => m.Document).ToList();


            return View(vm);
        }


        public ActionResult Suggest(bool highlights, bool fuzzy, string term)
        {
            InitSearch();

            // Call suggest API and return results
            var sp = new SuggestParameters()
            {
                UseFuzzyMatching = fuzzy,
                Top = 5
            };

            if (highlights)
            {
                sp.HighlightPreTag = "<b>";
                sp.HighlightPostTag = "</b>";
            }

            var resp = _indexClientWrapper.Documents.Suggest(term, "sg", sp);

            // Convert the suggest query results to a list that can be displayed in the client.
            var suggestions = resp.Results.Select(x => x.Text).ToList();
            return Json(suggestions);

        }


        public ActionResult AutoComplete(string term)
        {
            InitSearch();

            //Call autocomplete API and return results

            var ap = new AutocompleteParameters()
            {
                AutocompleteMode = AutocompleteMode.OneTermWithContext,
                UseFuzzyMatching = false,
                Top = 5
            };
            var autocompleteResult = _indexClientWrapper.Documents.Autocomplete(term, "sg", ap);

            // Conver the Suggest results to a list that can be displayed in the client.
            var autocomplete = autocompleteResult.Results.Select(x => x.Text).ToList();
            return Json(autocomplete);

        }


    }
}         

         */
        #endregion

    }
}