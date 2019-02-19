using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GDSHelpers.TagHelpers
{

    [HtmlTargetElement("gds-back-link", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class BackLinkHelper : TagHelper
    {
        [HtmlAttributeName("link-text")]
        public string LinkText { get; set; }

        [HtmlAttributeName("url")]
        public string Url { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";
            output.Attributes.SetAttribute("class", "govuk-back-link");
            output.Attributes.SetAttribute("href", Url);
            output.Content.SetContent(LinkText);

        }
    }
}
