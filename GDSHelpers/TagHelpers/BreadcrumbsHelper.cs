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
            sb.AppendLine("<ol class=\"govuk-breadcrumbs__list\">");

            var last = Breadcrumbs.Crumbs.Last();
            foreach (var crumb in Breadcrumbs.Crumbs)
            {
                sb.AppendLine($"<li class=\"govuk-breadcrumbs__list-item\" { (crumb.Equals(last) ? "aria-current=\"page\"" : "")}>");
                if (!crumb.Equals(last)) sb.AppendLine($"<a class=\"govuk-breadcrumbs__link\" href=\"{crumb.Url}\">");
                sb.AppendLine($"{crumb.Text}");
                if (!crumb.Equals(last)) sb.AppendLine("</a>");
                sb.AppendLine("</li>");
            }

            sb.AppendLine("</ol>");

            output.Content.SetHtmlContent(sb.ToString());

        }
    }
}
