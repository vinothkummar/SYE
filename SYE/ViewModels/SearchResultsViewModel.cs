﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SYE.Models;
using SYE.Services;

namespace SYE.ViewModels
{
    public class SearchResultsViewModel : PageModel
    {
        public SearchResultsViewModel()
        {
            Search = string.Empty;
            Facets = new List<string>();
        }

        [BindProperty(SupportsGet = true)]

        public bool ShowResults { get; set; }
        public int CurrentPage { get; set; } = 1;

        public long Count { get; set; }
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public bool ShowPrev => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;

        public string Search { get; set; }
        public List<SearchResult> Data { get; set; }
        public List<String> Facets { get; set; }
    }
}
