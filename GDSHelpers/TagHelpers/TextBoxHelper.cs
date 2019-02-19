using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GDSHelpers.TagHelpers
{

    [HtmlTargetElement("gds-text-box", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class TextBoxHelper : TagHelper
    {
        private readonly IHtmlGenerator _htmlGenerator;
        private readonly HtmlEncoder _htmlEncoder;
        
        public TextBoxHelper(IHtmlGenerator htmlGenerator, HtmlEncoder htmlEncoder)
        {
            _htmlGenerator = htmlGenerator;
            _htmlEncoder = htmlEncoder;
        }

        [HtmlAttributeName("for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            
            ViewContext.ViewData.ModelState.TryGetValue(For.Name, out var entry);
            var cssClass = entry?.Errors?.Count > 0 ? "govuk-form-group govuk-form-group--error" : "govuk-form-group";
            output.Attributes.Add("class", cssClass);

            var modelBuilder = new ModelBuilder
            {
                For = For,
                ViewContext = ViewContext,
                HtmlEncoder = _htmlEncoder,
                HtmlGenerator = _htmlGenerator
            };
            
            using (var writer = new StringWriter())
            {
                modelBuilder.WriteLabel(writer);

                if (!string.IsNullOrEmpty(For.Metadata.Description))
                    modelBuilder.WriteHint(writer);

                modelBuilder.WriteTextBox(writer);
                modelBuilder.WriteValidation(writer);
                output.Content.SetHtmlContent(writer.ToString());
            }

        }
        
    }
}
