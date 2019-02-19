using System.Linq;
using System.Text;
using GDSHelpers.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GDSHelpers.TagHelpers
{

    [HtmlTargetElement("gds-bread-crumbs", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class BreadcrumbsHelper : TagHelper
    {
        [HtmlAttributeName("bread-crumbs")]
        public BreadCumbs Breadcrumbs { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.Attributes.SetAttribute("class", "govuk-breadcrumbs");

            var sb = new StringBuilder();
            sb.Append("<ol class=\"govuk-breadcrumbs__list\">");

            var last = Breadcrumbs.Crumbs.Last();
            foreach (var crumb in Breadcrumbs.Crumbs)
            {
                sb.Append($"<li class=\"govuk-breadcrumbs__list-item\" { (crumb.Equals(last) ? "aria-current=\"page\"" : "")}>");
                if (!crumb.Equals(last)) sb.Append($"<a class=\"govuk-breadcrumbs__link\" href=\"{crumb.Url}\">");
                sb.Append($"{crumb.Text}");
                if (!crumb.Equals(last)) sb.Append("</a>");
                sb.Append("</li>");
            }

            sb.Append("</ol>");

            output.Content.SetHtmlContent(sb.ToString());

        }
    }
}
