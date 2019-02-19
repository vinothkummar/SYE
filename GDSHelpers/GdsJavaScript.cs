using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GDSHelpers
{
    public static class GdsJavaScript
    {
        public static IHtmlContent GdsWriteJavaScript(this IHtmlHelper helper)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<script>");




            sb.AppendLine("</script>");

            return new HtmlString(sb.ToString());
        }


    }
}
