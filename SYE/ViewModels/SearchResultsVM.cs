using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SYE.Models;
using SYE.Services;

namespace SYE.ViewModels
{
    public class SearchResultsVM    // : PageModel
    {
        public SearchResultsVM()
        {
            Search = string.Empty;
            //Facets = new List<string>();
        }

        [BindProperty(SupportsGet = true)]

        public bool ShowResults { get; set; }
        public int CurrentPage { get; set; } = 1;

        public long Count { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public bool ShowPrev => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;

        [Required(ErrorMessage = "Please enter a search")]
        public string Search { get; set; }
        public List<SearchResult> Data { get; set; }
        public List<SelectListItem> Facets { get; set; }

        public string SelectedFacets
        {
            get { return string.Join(',', this.Facets.Where(x => x.Selected).Select(x => x.Text).ToList()); }
        }
        public IEnumerable<string> TypeOfService { get; set; }
    }
}
