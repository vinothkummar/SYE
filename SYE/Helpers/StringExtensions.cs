using System.Text.RegularExpressions;

namespace SYE.Helpers
{
    public static class StringExtensions
    {
        public static string StripHtml(this string input)
        {
            return Regex.Replace(input, "<.*?>", string.Empty);
        }

        public static string RemoveLineBreaks(this string input)
        {
            return input.Replace("\r\n", string.Empty);
        }
    }
}
