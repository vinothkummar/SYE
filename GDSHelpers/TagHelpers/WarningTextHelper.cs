using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GDSHelpers.TagHelpers
{

    [HtmlTargetElement("gds-warning-text", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class WarningTextHelper : TagHelper
    {
        [HtmlAttributeName("message")]
        public string Message { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "govuk-warning-text");

            var sb = new StringBuilder();
            sb.AppendLine("<span class=\"govuk-warning-text__icon\" aria-hidden=\"true\">!</span>");
            sb.AppendLine("<strong class=\"govuk-warning-text__text\">");
            sb.AppendLine("<span class=\"govuk-warning-text__assistive\">Warning</span>");
            sb.AppendLine(Message);
            sb.AppendLine("</strong>");
            
            output.PostContent.SetHtmlContent(sb.ToString());
        }
    }
}
