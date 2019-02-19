﻿using System;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GDSHelpers.TagHelpers
{

    [HtmlTargetElement("gds-button", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class ButtonHelper : TagHelper
    {
        public ButtonHelper()
        {
            ButtonStatus = GdsEnums.Status.Enabled;
        }

        [HtmlAttributeName("button-type")]
        public GdsEnums.Buttons ButtonType { get; set; }

        [HtmlAttributeName("button-text")]
        public string ButtonText { get; set; }
        
        [HtmlAttributeName("button-status")]
        public GdsEnums.Status ButtonStatus { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "button ";
            output.Attributes.SetAttribute("class", "govuk-button");
            output.Attributes.SetAttribute("type", ButtonType.ToString().ToLower());

            if (ButtonStatus == GdsEnums.Status.Disabled)
            {
                output.Attributes.SetAttribute("disabled", "disabled");
                output.Attributes.SetAttribute("aria-disabled", "true");
                output.Attributes.SetAttribute("class", "govuk-button govuk-button--disabled");
            }

            output.Content.SetContent(ButtonText);

        }
    }
}
