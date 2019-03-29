using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SYE.Helpers
{
    public static class ViewHelper
    {
        /// <summary>
        /// converts a list of strings to a list of SelectListItems
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<SelectListItem> ConvertList(IEnumerable<string> source)
        {
            var indexer = 0;
            var returnList = source.Select(x => new SelectListItem {Text = x, Selected = false, Value = indexer++.ToString() }).ToList();

            return returnList;
        }
    }
}
