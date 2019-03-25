using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using FluentAssertions;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Moq;
using SYE.Services;
using SYE.Services.Wrappers;
using Xunit;

namespace SYE.Tests.Services
{
    public class SearchServiceTests
    {
        [Fact]
        public void Search_Should_Get_Result()
        {
            //arrange
            var expectedResult = new Models.SearchResult
            {
                Id = "testid",
                Name = "test location",
                Address = "test address",
                PostCode = "12345",
                Town = "test town",
                Region = "test region",
                Category = "test category"
            };
            var doc = new Document
            {
                {"rid", expectedResult.Id},
                {"locationName", expectedResult.Name},
                {"postalAddressLine1", expectedResult.Address},
                {"postalAddressTownCity", expectedResult.Town},
                {"postalCode", expectedResult.PostCode},
                {"region", expectedResult.Region},
                {"inspectionDirectorate", "test category"}
            };

            var mockedIndexClient = new Mock<ICustomSearchIndexClient>();
            var documentResult = new DocumentSearchResult
            {
                Results = new List<SearchResult> { new SearchResult { Document = doc } }
            };

            mockedIndexClient.Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<SearchParameters>())).ReturnsAsync(documentResult);

            //act
            var sut = new SearchService(mockedIndexClient.Object);
            var result = sut.GetPaginatedResult("searchString", 1, 10).Result[0];

            //assert
            result.Id.Should().Be(expectedResult.Id);
            result.Name.Should().Be(expectedResult.Name);
            result.Address.Should().Be(expectedResult.Address);
            result.Town.Should().Be(expectedResult.Town);
            result.PostCode.Should().Be(expectedResult.PostCode);
            result.Region.Should().Be(expectedResult.Region);
            result.Category.Should().Be(expectedResult.Category);
        }
        [Fact]
        public void Search_Should_Get_One_Record()
        {
            //arrange
            var setupResult = new Models.SearchResult
            {
                Id = "testid",
                Name = "test location",
                Address = "test address",
                PostCode = "12345",
                Town = "test town",
                Region = "test region",
                Category = "test category"
            };
            var doc = new Document
            {
                {"rid", setupResult.Id},
                {"locationName", setupResult.Name},
                {"postalAddressLine1", setupResult.Address},
                {"postalAddressTownCity", setupResult.Town},
                {"postalCode", setupResult.PostCode},
                {"region", setupResult.Region},
                {"inspectionDirectorate", "test category"}
            };

            var mockedIndexClient = new Mock<ICustomSearchIndexClient>();
            var documentResult = new DocumentSearchResult
            {
                Results = new List<SearchResult> { new SearchResult { Document = doc } }
            };

            mockedIndexClient.Setup(x => x.SearchAsync(It.IsAny<string>(), It.IsAny<SearchParameters>())).ReturnsAsync(documentResult);

            //act
            var sut = new SearchService(mockedIndexClient.Object);
            var result = sut.GetPaginatedResult("searchString", 1, 10).Result;

            //assert
            result.Count.Should().Be(1);
        }
    }
}