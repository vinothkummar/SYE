using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GDSHelpers.TagHelpers
{

    [HtmlTargetElement("gds-header-service-name", ParentTag = "gds-header-content")]
    public class HeaderServiceNameHelper : TagHelper
    {
        [HtmlAttributeName("url")]
        public string Url { get; set; }

        [HtmlAttributeName("title")]
        public string Title { get; set; }

        [HtmlAttributeName("service-name")]
        public string ServiceName { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            output.Attributes.SetAttribute("class", "govuk-header__link govuk-header__link--service-name");
            output.Attributes.SetAttribute("href", Url);

            if(!string.IsNullOrEmpty(Title)) output.Attributes.SetAttribute("title", Title);

            output.Content.SetContent(ServiceName);
        }
    }

}
