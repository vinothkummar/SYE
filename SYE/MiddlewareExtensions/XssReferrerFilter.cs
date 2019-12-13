﻿using System;
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
            var refererUrl = context.HttpContext.Request.Headers["Referer"].ToString();

            var filteredUrl = CheckForXssInHeader(refererUrl, _replacementReferer);

            context.HttpContext.Request.Headers["Referer"] = filteredUrl;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            //do nothing
            //throw new NotImplementedException();
        }

        public string CheckForXssInHeader(string refererUrl, string replacementReferrer)
        {
            string cleanRefererUrl = refererUrl.StripHtml();

            if (_restrictedWords != null)
                foreach (var word in _restrictedWords.Where(word => cleanRefererUrl.Contains(word)))
                {
                    return replacementReferrer;
                }

            if (_disallowedChars != null)
                foreach (var character in _disallowedChars.Where(character => cleanRefererUrl.Contains(character)))
                {
                    return replacementReferrer;
                }

            return cleanRefererUrl;
        }
    }
}
