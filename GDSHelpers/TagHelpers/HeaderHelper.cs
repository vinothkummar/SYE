using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GDSHelpers.TagHelpers
{

    [HtmlTargetElement("gds-header", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class HeaderHelper : TagHelper
    {
        public HeaderHelper()
        {
            Caption = "";
        }

        [HtmlAttributeName("header")]
        public GdsEnums.Headers Header { get; set; }

        [HtmlAttributeName("text")]
        public string Text { get; set; }

        [HtmlAttributeName("caption")]
        public string Caption { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var tag = Header.ToString().ToLower();
            var cssClass = GdsEnums.GetCssClassFromEnum(Header);
            
            output.TagName = tag;
            output.Attributes.SetAttribute("class", cssClass);

            if (!string.IsNullOrEmpty(Caption))
            {
                var caption = new TagBuilder("span");
                caption.MergeAttribute("class", cssClass.Replace("heading", "caption"));
                caption.InnerHtml.Append(Caption);
                output.PreContent.SetHtmlContent(caption);
            }

            output.Content.SetContent(Text);
        }
    }
}
