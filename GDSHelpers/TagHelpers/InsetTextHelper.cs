using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GDSHelpers.TagHelpers
{

    [HtmlTargetElement("gds-inset-text", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class InsetTextHelper : TagHelper
    {
        [HtmlAttributeName("content")]
        public string Content { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "govuk-inset-text");
            output.Content.SetContent(Content);
        }
    }
}
