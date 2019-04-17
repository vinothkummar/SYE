using FluentAssertions;
using SYE.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SYE.Tests.Helpers
{
    public class SubmissionHelperTests
    {
        [Theory]
        [InlineData("12345", "67890", 4, "2345-7890")]
        [InlineData("abcd", "efgh", 4, "ABCD-EFGH")]
        [InlineData("a", "b", 4, "A-B")]
        [InlineData("ab", "cd", 4, "AB-CD")]
        [InlineData("12345abc", "67890def", 4, "5ABC-0DEF")]
        [InlineData("111111111111111111111234", "2222222222222225678", 4, "1234-5678")]
        [InlineData("1", "2", 4, "1-2")]
        [InlineData("", "", 4, "")]
        [InlineData("********************", "", 4, "")]
        [InlineData("", "********************", 4, "")]
        [InlineData(null, null, 4, "")]
        public void GenerateUserRefTest(string string1, string string2, int numChars, string expectedResult)
        {
            var result = SubmissionHelper.GenerateUserRef(string1, string2, numChars);
            result.Should().Be(expectedResult);
        }
    }
}
