using System.Collections.Generic;
using SYE.Models;

namespace SYE.ViewModels
{
    public class SearchResultsViewModel
    {
        public string Search { get; set; }
        public List<SearchResult> SearchResults { get; set; }

        public int ResultsCount => SearchResults?.Count ?? 0;
    }
}
