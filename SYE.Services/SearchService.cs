using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using SYE.Models;
using SYE.Repository;
using SYE.Services.Helpers;
using SYE.Services.Wrappers;
using SearchResult = SYE.Models.SearchResult;

namespace SYE.Services
{
    public class SearchServiceResult
    {
        public long Count { get; set; }
        public IList<string> Facets { get; set; }
        public IList<SearchResult> Data { get; set; }
    }

    public interface ISearchService
    {
        Task<SearchServiceResult> GetPaginatedResult(string search, int currentPage, int pageSize, string refinementFacets, bool newSearch);
    }

    public class SearchService : ISearchService
    {
        private readonly ICustomSearchIndexClient _indexClientWrapper;
        private static IList<string> _facets = new List<string>();

        public SearchService(ICustomSearchIndexClient indexClientWrapper)
        {
            _indexClientWrapper = indexClientWrapper;
        }

        public async Task<SearchServiceResult> GetPaginatedResult(string search, int currentPage, int pageSize, string refinementFacets, bool newSearch)
        {
            return await GetDataAsync(search, currentPage, pageSize, refinementFacets, newSearch);
        }

        #region Private Methods
        private async Task<SearchServiceResult> GetDataAsync(string search, int currentPage, int pageSize, string refinementFacets, bool newSearch)
        {
            if (newSearch)
            {
                //clear out all facets for previous search
                _facets.Clear();
                refinementFacets = string.Empty;
            }

            var sp = new SearchParameters
            {
                IncludeTotalResultCount = true,
                Skip = ((currentPage - 1) * pageSize),
                Top = pageSize,
                Facets = new List<String> { "syeInspectionCategories,count:100" }
            };

            sp.Filter = "registrationStatus eq 'Registered'";
            if (!string.IsNullOrWhiteSpace(refinementFacets))
            {
                sp.Filter = string.Concat(sp.Filter, " and (", SearchHelper.BuildFilter(refinementFacets), ")");
            }

            var searchResult = await _indexClientWrapper.SearchAsync(search, sp).ConfigureAwait(false);

            if (searchResult.Facets?.Count == 1)
            {
                foreach (var item in searchResult?.Facets?.FirstOrDefault().Value?.Select(x => x.Value.ToString().Trim()))
                {
                    if (!_facets.Contains(item))
                    {
                        _facets.Add(item);
                    }
                }
            }

            return
                new SearchServiceResult()
                {
                    Count = searchResult.Count ?? 0,
                    Facets = _facets,
                    Data = searchResult?.Results?.Select(x => SearchHelper.GetSearchResult(x.Document))?.ToList() ?? new List<SearchResult>()
                };
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