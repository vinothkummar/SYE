using System.Collections.Generic;

namespace SYE.ViewModels.CheckYourAnswers
{
    public class CheckAnswersVM
    {
        public string LocationName { get; set; }

        public bool SendConfirmationEmail { get; set; }

        public List<CheckPageVM> Pages { get; set; }

        public List<string> SkipPages { get; set; }
    }
}
