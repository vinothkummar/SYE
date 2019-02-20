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
            sb.Append("<p class=\"govuk-phase-banner__content\">");

            sb.Append($"<strong class=\"govuk-tag govuk-phase-banner__content__tag\">{Phase.ToString()}</strong>");

            sb.Append("<span class=\"govuk-phase-banner__text\">This is a new service – your ");
            sb.Append($"<a class=\"govuk-link\" href=\"{Url}\">feedback</a>");
            sb.Append(" will help us to improve it.</span>");

            sb.Append("</p>");
            
            output.PostContent.SetHtmlContent(sb.ToString());

        }
    }
}
