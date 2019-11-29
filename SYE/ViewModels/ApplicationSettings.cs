using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYE.ViewModels
{
    public class GetHelp
    {
        public string ContactNumber { get; set; }
        public string ContactHours { get; set; }
        public string ContactExcluding { get; set; }
    }

    public class GFCUrls
    {
        public string StartPage { get; set; }
        public string RedirectUrl { get; set; }
    }

    public class ApplicationSettings
    {
        public string AppName { get; set; }
        public string FormStartPage { get; set; }
        public string ServiceNotFoundPage { get; set; }
        public string DefaultBackLink { get; set; }
        public GetHelp GetHelp { get; set; }
        public string AllowedCorsDomains { get; set; }
        public GFCUrls GFCUrls { get; set; }
    }
}
