using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.Search.Models;
using SearchResult = SYE.Models.SearchResult;

namespace SYE.Services.Helpers
{
    public static class SearchHelper
    {
        /// <summary>
        /// Generates a list of SearchResult objects form a list of Document results
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static List<SearchResult> ConvertResults(List<Document> results)
        {
            var returnList = new List<SearchResult>();
            if (results != null)
            {
                returnList = results.Select(doc => GetSearchResult(doc)).ToList();
            }

            return returnList;
        }

        public static string BuildFilter(string refinementFacets)
        {
            string filter = null;
            var facets = refinementFacets.Split(',');

            for (var index = 0; index < facets.Length; index++)
            {
                var facet = facets[index];
                if (index == 0)
                {
                    filter = "inspectionDirectorate eq '" + facet + "'";
                }
                else
                {
                    filter += " or inspectionDirectorate eq '" + facet + "'";
                }
            }

            return filter;
        }

        /// <summary>
        /// Generates a SearchResult object from a document
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        private static SearchResult GetSearchResult(Document doc)
        {
            var searchResult = new SearchResult
            {
                Id = GetValue(doc, "id"),
                Name = GetValue(doc, "locationName"),
                Address = GetValue(doc, "postalAddressLine1"),
                Address2 = GetValue(doc, "postalAddressLine2"),
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
        private static string GetValue(Document doc, string key)
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

    }
}
