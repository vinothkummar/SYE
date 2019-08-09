using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYE.ViewModels
{
    public class ValidationActions
    {
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
        public bool ShowIncompletedSearchMessage { get; set; }
        public bool ShowExceededMaxLengthMessage { get; set; }
    }
}
