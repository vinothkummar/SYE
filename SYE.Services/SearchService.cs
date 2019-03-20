using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using SYE.Models;
using SYE.Repository;
using SearchResult = SYE.Models.SearchResult;

namespace SYE.Services
{
    public interface ISearchService
    {
        List<SearchResult> GetServiceProviders(string search);
        ServiceProvider GetServiceProvider(string id);
    }
    public class SearchService : ISearchService
    {
        private static SearchServiceClient _searchClient;
        private static ISearchIndexClient _indexClient;
        private static string _indexName = "documentdb-index";

        private readonly IGenericRepository<SearchResult> _genericRepository;

        public SearchService(IGenericRepository<SearchResult> genericRepository)
        {
            _genericRepository = genericRepository;
        }
        public List<SearchResult> GetServiceProviders(string search)
        {
            var sp = new SearchParameters();

            InitSearch();

            var results = _indexClient.Documents.Search(search, sp).Results.Select(m => m.Document).ToList();

            var pagedResults = results.Skip(0).Take(20).ToList();

            return ConvertResults(results);
        }

        public ServiceProvider GetServiceProvider(string id)
        {
            throw new NotImplementedException();
        }

        private void InitSearch()
        {
            var searchServiceName = "sye-poc-azure-search";
            var apiKey = "260467EC7EE731A6CCC5CFDBD97D5D99";

            _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            _indexClient = _searchClient.Indexes.GetClient(_indexName);
        }

        /// <summary>
        /// Generates a list of SearchResult objects form a list of Document results
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private List<SearchResult> ConvertResults(IReadOnlyCollection<Document> results)
        {
            var returnList = new List<SearchResult>();
            if (results != null)
            {
                returnList = results.Select(doc => GetSearchResult(doc)).ToList();
            }

            return returnList;
        }

        /// <summary>
        /// Generates a SearchResult object from a document
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private SearchResult GetSearchResult(Document doc)
        {
            var searchResult = new SearchResult
            {
                Id = GetValue(doc, "rid"),
                Name = GetValue(doc, "locationName"),
                Address = GetValue(doc, "postalAddressLine1"),
                Town = GetValue(doc, "postalAddressTownCity"),
                PostCode = GetValue(doc, "postalCode"),
                Region = GetValue(doc, "region"),
                Category = GetValue(doc, "inspectionDirectorate")
            };
            return searchResult;
        }

        /// <summary>
        /// gets a single field value from a document
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetValue(Document doc, string key)
        {
            string returnStr = null;

            if (doc.ContainsKey(key))
            {
                doc.TryGetValue(key, out var obj);
                if (obj != null)
                {
                    returnStr = obj.ToString();
                }                
            }
            return returnStr;
        }

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
        private static ISearchIndexClient _indexClient;
        private static string _indexName = "sye-poc-index";

        public static string errorMessage;

        private void InitSearch()
        {
            var searchServiceName = "sye-poc-azure-search";
            var apiKey = "636F112CE66183702BCB07925E6EBB59";

            _searchClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            _indexClient = _searchClient.Indexes.GetClient(_indexName);
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

            var results = _indexClient.Documents.Search(vm.SearchTerm, sp).Results.Select(m => m.Document).ToList();


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

            var resp = _indexClient.Documents.Suggest(term, "sg", sp);

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
            var autocompleteResult = _indexClient.Documents.Autocomplete(term, "sg", ap);

            // Conver the Suggest results to a list that can be displayed in the client.
            var autocomplete = autocompleteResult.Results.Select(x => x.Text).ToList();
            return Json(autocomplete);

        }


    }
}         

         */

    }
}