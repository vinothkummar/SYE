namespace SYE.Helpers.Enums
{
    public enum EnumStatusCode
    {
        //search errors
        SearchUnavailableError = 550,
        SearchSelectLocationJsonError = 551,
        SearchLocationNotFoundJsonError = 552,
        //Report a problem errors
        RPPageLoadJsonError = 555,
        RPSubmissionJsonError = 556,
        //Questionnaire journey errors        
        FormPageLoadSessionNullError = 560,
        FormPageLoadLocationNullError = 561,
        FormPageLoadNullError = 562,
        FormPageContinueSessionNullError = 563,
        FormPageContinueNullError = 564,
        FormContinueLocationNullError = 565,
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