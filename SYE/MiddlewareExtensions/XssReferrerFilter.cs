using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using GDSHelpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SYE.Helpers;

namespace SYE.MiddlewareExtensions
{
    public class XssReferrerFilter : IResourceFilter
    {
        private string _refererUrl;
        private string _filteredUrl;

        private IConfiguration _config;
        private string _replacementReferer;

        private static readonly HashSet<char> _disallowedChars = new HashSet<char>(@"\<>'\""&");
        private static readonly List<string> _restrictedWords = new List<string> { "javascript", "onblur", "onchange", "onfocus", "onfocusin", "onfocusout", "oninput", "onmouseenter", "onmouseleave",
            "onselect", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onscroll", "ontouchstart",
            "ontouchend", "ontouchmove", "ontouchcancel", "onwheel"};

        public XssReferrerFilter(IConfiguration Config)
        {
            _config = Config;
            _replacementReferer = "http://dev.cqc.org.uk/give-feedback-on-care";
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            _refererUrl = context.HttpContext.Request.Headers["Referer"].ToString();

            _filteredUrl = CheckForXssInHeader(_refererUrl, _replacementReferer, _disallowedChars, _restrictedWords);

            context.HttpContext.Request.Headers["Referer"] = _filteredUrl;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            //do nothing
            //throw new NotImplementedException();
        }

        public string CheckForXssInHeader(string RefererUrl, string ReplacementReferrer, HashSet<char> DisallowedChars, List<string> RestrictedWords)
        {
            HashSet<char> disallowedChars = DisallowedChars;
            List<string> restrictedWords = RestrictedWords;
            string refererUrl = RefererUrl;

            refererUrl = refererUrl.StripHtml();

            if (restrictedWords != null)
                foreach (var word in restrictedWords.Where(word => refererUrl.Contains(word)))
                {
                    return ReplacementReferrer;
                }

            if (disallowedChars != null)
                foreach (var character in disallowedChars.Where(character => refererUrl.Contains(character)))
                {
                    return ReplacementReferrer;
                }

            return refererUrl;
        }
    }
}
