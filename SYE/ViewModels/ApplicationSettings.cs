﻿
namespace SYE.ViewModels
{
    public class GetHelp
    {
        public string ContactNumber { get; set; }
        public string ContactHours { get; set; }
        public string ContactExcluding { get; set; }
    }

    public class ApplicationSettings
    {
        public string AppName { get; set; }
        public string FormStartPage { get; set; }
        public string ServiceNotFoundPage { get; set; }
        public string DefaultBackLink { get; set; }
        public GetHelp GetHelp { get; set; }
    }
}
