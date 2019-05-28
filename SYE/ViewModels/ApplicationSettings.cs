using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYE.ViewModels
{
    public class ApplicationSettings
    {
        public string AppName { get; set; }
        public string FormStartPage { get; set; }
        public string ServiceNotFoundPage { get; set; }
        public string DefaultBackLink { get; set; }
        public string UsersNameField { get; set; }
        public string UsersEmailField { get; set; }
    }
}
