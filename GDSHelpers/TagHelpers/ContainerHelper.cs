using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GDSHelpers.TagHelpers
{

    [HtmlTargetElement("gds-container")]
    public class ContainerHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "main";
            output.Attributes.SetAttribute("class", "govuk-width-container");
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }

}
