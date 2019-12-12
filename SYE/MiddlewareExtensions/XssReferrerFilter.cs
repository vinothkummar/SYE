using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using GDSHelpers;
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


        private static readonly HashSet<char> _allowedChars = new HashSet<char>(@"1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz.,'()?!#&$£%^@*;:+=_-/ ");
        private static readonly List<string> _restrictedWords = new List<string> { "javascript", "onblur", "onchange", "onfocus", "onfocusin", "onfocusout", "oninput", "onmouseenter", "onmouseleave",
            "onselect", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onscroll", "ontouchstart",
            "ontouchend", "ontouchmove", "ontouchcancel", "onwheel" };

        public XssReferrerFilter(IConfiguration Config)
        {
            _config = Config;
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            //_filteredUrl = _refererUrl = context.HttpContext.Request.Headers["Referer"].ToString();
            //_filteredUrl = _refererUrl = "http://localhost:60045/\"<a onmouseover=\"alert(33)\">";

            //if (string.IsNullOrEmpty(_refererUrl))
            //    _filteredUrl = string.Empty;

            //if (_restrictedWords != null)
            //    foreach (var word in _restrictedWords.Where(word => _filteredUrl.Contains(word)))
            //    {
            //        _filteredUrl.Replace(word, "");
            //        //_filteredUrl = _config.GetSection("ApplicationSettings").GetValue<String>("LandingPage");
            //    }

            //if (_allowedChars != null)
            //    _filteredUrl = string.Concat(_filteredUrl.Where(c => _allowedChars.Contains(c)));

            //_filteredUrl.StripHtml();

            //context.HttpContext.Request.Headers["Referer"] = _filteredUrl;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            //do nothing
            //throw new NotImplementedException();
        }
    }
}
