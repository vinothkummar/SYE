using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYE.Models
{
    public class SearchResultsViewModel
    {
        public string Search { get; set; }
        public List<SearchResult> SearchResults { get; set; }
    }
}
