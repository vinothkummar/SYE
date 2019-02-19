using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GDSHelpers.TagHelpers
{

    [HtmlTargetElement("gds-column", ParentTag = "gds-row")]
    public class ColumnHelper : TagHelper
    {
        [HtmlAttributeName("desktop-size")]
        public GdsEnums.DesktopColumns DesktopSize { get; set; }

        [HtmlAttributeName("tablet-size")]
        public GdsEnums.TabletColumns TabletSize { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var tabletClass = GdsEnums.GetCssClassFromEnum(TabletSize);
            var desktopClass = GdsEnums.GetCssClassFromEnum(DesktopSize);

            output.TagName = "div";
            output.Attributes.SetAttribute("class", $"{tabletClass} {desktopClass}");
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }

}
