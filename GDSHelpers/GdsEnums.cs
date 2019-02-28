using System;
using System.ComponentModel;

namespace GDSHelpers
{
    public class GdsEnums
    {
        public enum CountTypes
        {
            None,
            Characters,
            Words,
        }

        public enum Phases
        {
            Alpha,
            Beta,
        }

        public enum TabletColumns
        {
            [Description("govuk-grid-column-full")]
            FullWidth,

            [Description("govuk-grid-column-one-half")]
            OneHalf,

            [Description("govuk-grid-column-one-third")]
            OneThird,

            [Description("govuk-grid-column-two-thirds")]
            TwoThirds,

            [Description("govuk-grid-column-one-quarter")]
            OneQuarter,

            [Description("govuk-grid-column-three-quarters")]
            ThreeQuarter
        }

        public enum DesktopColumns
        {
            [Description("govuk-grid-column-full-from-desktop")]
            FullWidth,

            [Description("govuk-grid-column-one-half-from-desktop")]
            OneHalf,

            [Description("govuk-grid-column-one-third-from-desktop")]
            OneThird,

            [Description("govuk-grid-column-two-thirds-from-desktop")]
            TwoThirds,

            [Description("govuk-grid-column-one-quarter-from-desktop")]
            OneQuarter,

            [Description("govuk-grid-column-three-quarters-from-desktop")]
            ThreeQuarter
        }

        public enum Buttons
        {
            Button,
            Submit
        }

        public enum Status
        {
            [Description("")]
            Enabled,

            [Description("govuk-button--disabled")]
            Disabled,
        }

        public enum Headers
        {
            [Description("govuk-heading-xl")]
            H1,

            [Description("govuk-heading-l")]
            H2,

            [Description("govuk-heading-m")]
            H3,

            [Description("govuk-heading-s")]
            H4
        }

        public enum Captions
        {
            [Description("govuk-caption-xl")]
            H1,

            [Description("govuk-caption-l")]
            H2,

            [Description("govuk-caption-m")]
            H3,

            [Description("govuk-caption-s")]
            H4
        }


        public static string GetCssClassFromEnum(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute),false);

            if (attributes.Length > 0) return attributes[0].Description;

            return value.ToString();
        }

    }
}
