namespace SYE.Helpers.Enums
{
    public enum EnumStatusCode
    {
        //search errors
        SearchPageLoadError = 550,//not used
        SearchUnavailableError = 551,
        SearchSelectLocationJsonError = 553,
        SearchLocationNotFoundJsonError = 554,
        SearchLocationNotFoundPageLoadError = 555,//not used
        //Report a problem errors
        RPPageLoadJsonError = 556,
        RPSubmissionJsonError = 557,
        RPEmailError = 558,//not used       
        //Questionnaire journey errors        
        FormPageLoadSessionNullError = 559,
        FormPageLoadLocationNullError = 560,
        FormPageLoadNullError = 561,
        FormPageContinueSessionNullError = 562,
        FormPageContinueNullError = 563,
        FormContinueLocationNullError = 564,
        FormPageUpdateError = 565,//not used
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