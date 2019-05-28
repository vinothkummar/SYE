using System.Collections.Generic;

namespace SYE.ViewModels.CheckYourAnswers
{
    public class CheckPageVM
    {
        public string PageId { get; set; }

        public List<CheckAnswerWM> Answers { get; set; }
    }
}
