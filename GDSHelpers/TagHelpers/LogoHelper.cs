using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GDSHelpers.TagHelpers
{

    [HtmlTargetElement("gds-logo", ParentTag = "gds-header-container")]
    public class LogoHelper : TagHelper
    {
        [HtmlAttributeName("url")]
        public string Url { get; set; }

        [HtmlAttributeName("url-title")]
        public string UrlTitle { get; set; }

        [HtmlAttributeName("alt-text")]
        public string AltText { get; set; }

        [HtmlAttributeName("image-url")]
        public string ImageUrl { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "govuk-header__logo");

            var urlTitle = string.IsNullOrEmpty(UrlTitle) ? "" : $"title=\"{UrlTitle}\"";
            var altText = string.IsNullOrEmpty(AltText) ? "" : $"alt=\"{AltText}\"";

            var sb = new StringBuilder();
            sb.AppendLine($"<a href=\"{Url}\" class=\"govuk-header__link govuk-header__link--homepage\" {urlTitle}>");
            sb.AppendLine($"<img src=\"{ImageUrl}\" class=\"govuk-header__logotype-crown-fallback-image\" {altText} />");
            sb.AppendLine("</a>");

            output.PostContent.SetHtmlContent(sb.ToString());
        }
    }

}
