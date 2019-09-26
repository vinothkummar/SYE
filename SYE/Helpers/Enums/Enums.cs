namespace SYE.Helpers.Enums
{
    public enum EnumStatusCode
    {
        //search errors
        SearchPageLoadError = 550,
        SearchUnavailableError = 551,
        SearchNotFoundJsonError = 553,
        SearchLocationSelectError = 554,
        SearchLocationNotFoundPageLoadError = 555,
        //Report a problem errors
        RPPageLoadJsonError = 556,
        //RPEmailError = 557,
        RPSubmissionJsonError = 558,
        //Questionnaire journey errors
        FormPageLoadError = 560,
        FormLocationNullError = 561,
        FormPageNullError = 562,
        FormSessionNullError = 563,
        FormSessionIncompleteError = 564,
        FormPageUpdateError = 565,
        //Check your answers errors
        CYAPageLoadError = 570,
        CYASessionNullError = 571,
        CYAFormNullError = 572,
        CYALocationNullError = 573,
        CYAFeedbackNullError = 574,
        CYASubmissionIdNullError = 575,
        CYASubmissionError = 576,
        CYASubmissionEmailError = 577,
        //ESB errors
        EsbSubmissionError = 578,
        EsbNotAvailable = 579
    }
}