using System.Linq;
using System.Text;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GDSHelpers
{
    public static class InlineHelpers
    {

        /// <summary>
        /// Creates a GDS compliant Question with label, help text and error messages
        /// </summary>
        /// <param name="helper">The HTML Class</param>
        /// <param name="question">The QuestionVM class</param>
        /// <param name="htmlAttributes">Any additional attributes to apply to the questions.</param>
        /// <returns>Returns the HTML for GDS compliant questions</returns>
        public static IHtmlContent GdsQuestion(this IHtmlHelper helper, QuestionVM question, object htmlAttributes = null)
        {
            switch (question.InputType)
            {
                case "textbox":
                    return BuildTextBox(question);

                case "textarea":
                    return BuildTextArea(question);

                case "optionlist":
                    return BuildOptionList(question);

                case "selectlist":
                    return BuildSelectList(question);

                case "checkboxlist":
                    return BuildCheckboxList(question);
            }
            return new TagBuilder("span");
        }



        private static IHtmlContent BuildTextBox(QuestionVM question)
        {
            var elementId = question.QuestionId;
            var isErrored = question.Validation?.IsErrored == true;
            var errorMsg = question.Validation?.ErrorMessage;
            var erroredCss = isErrored ? "govuk-form-group--error" : "";
            var erroredInputCss = isErrored ? "govuk-input--error" : "";

            var sb = new StringBuilder();

            sb.AppendLine($"<div class=\"govuk-form-group {erroredCss}\">");
            sb.AppendLine($"<label class=\"govuk-label\" for=\"{elementId}\">{question.Question}</label>");

            if (!string.IsNullOrEmpty(question.AdditionalText))
                sb.AppendLine($"<span id=\"{elementId}-hint\" class=\"govuk-hint\">{question.AdditionalText}</span>");

            if (isErrored)
                sb.AppendLine($"<span id=\"{elementId}-error\" class=\"govuk-error-message\">{errorMsg}</span>");

            sb.AppendLine($"<input class=\"govuk-input {erroredInputCss}\" id=\"{elementId}\" name=\"{elementId}\" type=\"{question.DataType}\" aria-describedby=\"{elementId}-hint\" value=\"{question.Answer}\">");

            sb.AppendLine("</div>");

            return new HtmlString(sb.ToString());
        }

        private static IHtmlContent BuildTextArea(QuestionVM question)
        {
            var elementId = question.QuestionId;
            var showCounter = question.Validation?.MaxLength != null;
            var counterCount = question.Validation?.MaxLength?.Max;
            var counterType = question.Validation?.MaxLength?.Type == "words" ? "maxwords" : "maxlength";
            var counterCss = showCounter ? "js-character-count" : "";

            var isErrored = question.Validation?.IsErrored == true;
            var errorMsg = question.Validation?.ErrorMessage;
            var erroredCss = isErrored ? "govuk-form-group--error" : "";
            var erroredInputCss = isErrored ? "govuk-textarea--error" : "";

            var sb = new StringBuilder();

            if (showCounter)
                sb.AppendLine($"<div class=\"govuk-character-count\" data-module=\"character-count\" data-{counterType}=\"{counterCount}\">");

            sb.AppendLine($"<div class=\"govuk-form-group {erroredCss}\">");
            sb.AppendLine($"<label class=\"govuk-label\"  for=\"{elementId}\">{question.Question}</label>");

            if (!string.IsNullOrEmpty(question.AdditionalText))
                sb.AppendLine($"<span id=\"{elementId}-hint\" class=\"govuk-hint\">{question.AdditionalText}</span>");

            if (isErrored)
                sb.AppendLine($"<span id=\"{elementId}-error\" class=\"govuk-error-message\">{errorMsg}</span>");

            sb.AppendLine($"<textarea class=\"govuk-textarea {erroredInputCss} {counterCss}\" id=\"{elementId}\" " +
                      $"name=\"{elementId}\" rows=\"5\" aria-describedby=\"{elementId}-hint\">{question.Answer}</textarea>");
            
            sb.AppendLine("</div>");


            if (showCounter)
            {
                sb.AppendLine($"<span id=\"{elementId}-info\" class=\"govuk-hint govuk-character-count__message\" aria-live=\"polite\"></span>");
                sb.AppendLine("</div>");
            }
            
            return new HtmlString(sb.ToString());
        }

        private static IHtmlContent BuildOptionList(QuestionVM question)
        {
            var elementId = question.QuestionId;
            var isErrored = question.Validation?.IsErrored == true;
            var errorMsg = question.Validation?.ErrorMessage;
            var erroredCss = isErrored ? "govuk-form-group--error" : "";


            var sb = new StringBuilder();

            sb.AppendLine($"<div class=\"govuk-form-group {erroredCss}\">");
            sb.AppendLine("<fieldset class=\"govuk-fieldset\" aria-describedby=\"changed-name-hint\">");

            sb.AppendLine("<legend class=\"govuk-fieldset__legend govuk-fieldset__legend--xl\">");
            sb.AppendLine($"<label class=\"govuk-label\">{question.Question}</label>");
            sb.AppendLine("</legend>");

            if (!string.IsNullOrEmpty(question.AdditionalText))
                sb.AppendLine($"<span id=\"{elementId}-hint\" class=\"govuk-hint\">{question.AdditionalText}</span>");

            if (isErrored)
                sb.AppendLine($"<span id=\"{elementId}-error\" class=\"govuk-error-message\">{errorMsg}</span>");


            var list = question.Options.Split(';');
            var inlineCSS = "";
            //list.Length < 3 ? "govuk-radios--inline" : "";
            sb.AppendLine($"<div class=\"govuk-radios {inlineCSS}\">");


            var count = 0;
            foreach (var item in list)
            {
                var optionArr = item.Split('|');
                var optionText = optionArr[0].Trim();
                var optionHint = "";
                if (optionArr.Length > 1)
                    optionHint = optionArr[1].Trim();

                var checkedCss = question.Answer == optionText ? "checked" : "";

                sb.AppendLine("<div class=\"govuk-radios__item\">");
                sb.AppendLine($"<input class=\"govuk-radios__input\" id=\"{elementId}-{count}\" name=\"{elementId}\" type=\"radio\" value=\"{optionText}\" {checkedCss}>");
                sb.AppendLine($"<label class=\"govuk-label govuk-radios__label\" for=\"{elementId}-{count}\">{optionText}</label>");

                if (optionArr.Length > 1)
                    sb.AppendLine($"<span id=\"{elementId}-{count}-item-hint\" class=\"govuk-hint govuk-radios__hint\">{optionHint}</span>");

                sb.AppendLine("</div>");
                count += 1;
            }

            sb.AppendLine("</div>");

            sb.AppendLine("</fieldset>");
            sb.AppendLine("</div>");

            return new HtmlString(sb.ToString());

        }

        private static IHtmlContent BuildSelectList(QuestionVM question)
        {
            var elementId = question.QuestionId;
            var isErrored = question.Validation?.IsErrored == true;
            var errorMsg = question.Validation?.ErrorMessage;
            var erroredCss = isErrored ? "govuk-form-group--error" : "";


            var sb = new StringBuilder();

            sb.AppendLine($"<div class=\"govuk-form-group {erroredCss}\">");
            sb.AppendLine($"<label class=\"govuk-label\" for=\"{elementId}\">{question.Question}</label>");

            if (!string.IsNullOrEmpty(question.AdditionalText))
                sb.AppendLine($"<span id=\"{elementId}-hint\" class=\"govuk-hint\">{question.AdditionalText}</span>");        

            if (isErrored)
                sb.AppendLine($"<span id=\"{elementId}-error\" class=\"govuk-error-message\">{errorMsg}</span>");


            var list = question.Options.Split(';');

            sb.AppendLine($"<select class=\"govuk-select\" id=\"{elementId}\" name=\"{elementId}\">");
            sb.AppendLine("<option value>Please select</option>");

            foreach (var item in list)
            {
                var isSelected = question.Answer == item ? "checked" : "";
                sb.AppendLine($"<option value=\"{item}\" {isSelected}>{item}</option>");
            }

            sb.AppendLine("</select>");

            sb.AppendLine("</div>");

            return new HtmlString(sb.ToString());

        }
        
        private static IHtmlContent BuildCheckboxList(QuestionVM question)
        {
            var elementId = question.QuestionId;
            var isErrored = question.Validation?.IsErrored == true;
            var errorMsg = question.Validation?.ErrorMessage;
            var erroredCss = isErrored ? "govuk-form-group--error" : "";


            var sb = new StringBuilder();

            sb.AppendLine($"<div class=\"govuk-form-group {erroredCss}\">");
            sb.AppendLine("<fieldset class=\"govuk-fieldset\" aria-describedby=\"changed-name-hint\">");

            sb.AppendLine("<legend class=\"govuk-fieldset__legend govuk-fieldset__legend--xl\">");
            sb.AppendLine($"<label class=\"govuk-label\">{question.Question}</label>");
            sb.AppendLine("</legend>");

            if (!string.IsNullOrEmpty(question.AdditionalText))
                sb.AppendLine($"<span id=\"{elementId}-hint\" class=\"govuk-hint\">{question.AdditionalText}</span>");

            if (isErrored)
                sb.AppendLine($"<span id=\"{elementId}-error\" class=\"govuk-error-message\">{errorMsg}</span>");


            var list = question.Options.Split(';');
            sb.AppendLine($"<div class=\"govuk-checkboxes\">");


            var count = 0;
            foreach (var item in list)
            {
                var optionArr = item.Split('|');
                var optionText = optionArr[0].Trim();
                var optionHint = "";
                if (optionArr.Length > 1)
                    optionHint = optionArr[1].Trim();

                var checkedCss = question.Answer.Split(',').Contains(optionText) ? "checked" : "";

                sb.AppendLine("<div class=\"govuk-checkboxes__item\">");
                sb.AppendLine($"<input class=\"govuk-checkboxes__input\" id=\"{elementId}-{count}\" name=\"{elementId}\" type=\"checkbox\" value=\"{optionText}\" {checkedCss}>");
                sb.AppendLine($"<label class=\"govuk-label govuk-checkboxes__label\" for=\"{elementId}-{count}\">{optionText}</label>");

                if (optionArr.Length > 1)
                    sb.AppendLine($"<span id=\"{elementId}-{count}-item-hint\" class=\"govuk-hint govuk-checkboxes__hint\">{optionHint}</span>");

                sb.AppendLine("</div>");
                count += 1;
            }

            sb.AppendLine("</div>");

            sb.AppendLine("</fieldset>");
            sb.AppendLine("</div>");

            return new HtmlString(sb.ToString());

        }
        
        public static IHtmlContent GdsButton(this IHtmlHelper helper, string buttonType, string buttonText, object htmlAttributes = null)
        {
            var button = new TagBuilder("button");
            button.Attributes.Add("class", "govuk-button");
            button.Attributes.Add("type", buttonType.ToLower());
            button.InnerHtml.Append(buttonText);

            button.MergeHtmlAttributes(htmlAttributes);

            return button;
        }





        private static void MergeHtmlAttributes(this TagBuilder tagBuilder, object htmlAttributes)
        {
            if (htmlAttributes != null)
            {
                var customAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                foreach (var customAttribute in customAttributes)
                {
                    tagBuilder.MergeAttribute(customAttribute.Key, customAttribute.Value.ToString());
                }
            }
        }

    }
}
