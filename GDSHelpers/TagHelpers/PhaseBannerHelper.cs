using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GDSHelpers.TagHelpers
{

    [HtmlTargetElement("gds-phase-banner", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class PhaseBannerHelper : TagHelper
    {
        [HtmlAttributeName("url")]
        public string Url { get; set; }

        [HtmlAttributeName("phase")]
        public GdsEnums.Phases Phase { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "govuk-phase-banner");

            var sb = new StringBuilder();
            sb.AppendLine("<p class=\"govuk-phase-banner__content\">");

            sb.AppendLine($"<strong class=\"govuk-tag govuk-phase-banner__content__tag\">{Phase.ToString()}</strong>");

            sb.AppendLine("<span class=\"govuk-phase-banner__text\">This is a new service – your ");
            sb.AppendLine($"<a class=\"govuk-link\" href=\"{Url}\">feedback</a>");
            sb.AppendLine(" will help us to improve it.</span>");

            sb.AppendLine("</p>");
            
            output.PostContent.SetHtmlContent(sb.ToString());

        }
    }
}
