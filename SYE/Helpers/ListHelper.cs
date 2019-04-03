using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SYE.Helpers
{
    public static class ListHelper
    {
        /// <summary>
        /// converts a list of strings to a list of SelectListItems
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<SelectItem> ConvertList(IEnumerable<string> source)
        {
            var returnList = source?.Select(x => new SelectItem {Text = x, Selected = false}).ToList();

            return returnList;
        }     
    }
}
