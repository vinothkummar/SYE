using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using SYE.Services;
using Xunit;

namespace SYE.Tests.Services
{
    public class PageServiceTests
    {
        private string _dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\Resources\\";
        private string _fileName = "form-schema.json";
        private string _location = "Test Location";

        [Theory]
        [InlineData(null, "What do you want to tell us about?")]
        [InlineData("", "What do you want to tell us about?")]
        [InlineData("aaaaaaaaa001", "What do you want to tell us about?")]
        [InlineData("aaaaaaaaa002", "Negative Path")]
        [InlineData("aaaaaaaaa101", "Tell us what happened")]
        [InlineData("aaaaaaaaa004", "Can we share the information you have given us with test location?")]
        [InlineData("aaaaaaaaa005", "Can we contact you")]
        [InlineData("aaaaaaaaa006", "Your contact details")]
        public void GetPageById_Should_Return_Correct_Data(string pageId, string expectedPagerName)
        {
            //arrange
            var sut = new PageService();
            //act            
            //TODO this will need to change to call the method to get from the cache            
            var result = sut.GetPageById(pageId, _dir + _fileName, "test location");

            //assert
            result.PageName.Should().Be(expectedPagerName);
        }

        [Theory]
        [InlineData(0, true)]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, false)]
        [InlineData(4, false)]
        public void GetPageById_Should_Return_Correct_Page(int pageIndex, bool getPageFromAnswerLogic)
        {
            //arrange
            var sut = new PageService();
            var happyPathPageList = GenerateHappyPathPageList();
            //act            
            var thisPageId = happyPathPageList[pageIndex];
            var expectedPageId = happyPathPageList[pageIndex + 1];

            //TODO this will need to change to call the method to get from the cache
            var result = sut.GetPageById(thisPageId, _dir + _fileName, _location);

            string next;

            if (getPageFromAnswerLogic)
            {
                next = result.Questions.ToList()[0].AnswerLogic.ToList()[0].NextPageId;
            }
            else
            {
                next = result.NextPageId;
            }

            //assert
            next.Should().Be(expectedPageId);
        }
        [Theory]
        [InlineData("aaaaaaaaa007")]
        [InlineData("xxxxxxxxxxxxxxxxxxxxx")]
        public void GetPageById_Should_Return_Null_Page(string pageId)
        {
            //arrange
            var sut = new PageService();
            //act            
            //TODO this will need to change to call the method to get from the cache
            var result = sut.GetPageById(pageId, _dir + _fileName, "test location");

            //assert
            result.Should().Be(null);
        }
        /// <summary>
        /// sets up a list of page ids representing the happy path
        /// </summary>
        private List<string> GenerateHappyPathPageList()
        {
            var happyPath = new List<string>
                {"aaaaaaaaa001", "aaaaaaaaa101", "aaaaaaaaa004", "aaaaaaaaa005", "aaaaaaaaa006", "aaaaaaaaa007"};
            return happyPath;
        }
 
    }
}
