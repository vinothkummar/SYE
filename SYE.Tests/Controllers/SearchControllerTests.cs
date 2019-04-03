using System;
using System.Collections.Generic;
using System.Text;
using Castle.Components.DictionaryAdapter;
using FluentAssertions;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SYE.Controllers;
using SYE.Models;
using SYE.Services;
using SYE.ViewModels;
using Xunit;

namespace SYE.Tests.Controllers
{
    public class SearchControllerTests
    {
        /*
        [Fact]
        public void SearchResultsShouldGetCorrectResult()
        {
            //arrange
            var expectedRecord = new Models.SearchResult
            {
                Id = "testid",
                Name = "test location",
                Address = "test address",
                PostCode = "12345",
                Town = "test town",
                Region = "test region",
                Category = "test category"
            };
            var expectedResult = new List<Models.SearchResult>();
            expectedResult.Add(expectedRecord);
            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(expectedResult).Verifiable();
                    
            //act
            var sut = new SearchController(mockService.Object, mockSession.Object);
            var result = sut.SearchResults("search");

            //assert
            var viewResult = result as ViewResult;

            var model = viewResult.Model as SearchResultsVM;
            model.ShowResults.Should().Be(true);
            var resultToCompare = model.Data[0];
            resultToCompare.Id.Should().Be(expectedRecord.Id);
            resultToCompare.Name.Should().Be(expectedRecord.Name);
            resultToCompare.Address.Should().Be(expectedRecord.Address);
            resultToCompare.PostCode.Should().Be(expectedRecord.PostCode);
            resultToCompare.Town.Should().Be(expectedRecord.Town);
            resultToCompare.Region.Should().Be(expectedRecord.Region);
            resultToCompare.Category.Should().Be(expectedRecord.Category);
            mockService.Verify();
        }

        [Fact]
        public void SearchResultsShouldReturnEmptyList()
        {
            //arrange
            List<SearchResult> expectedResult = null;
            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(expectedResult).Verifiable();

            //act
            var sut = new SearchController(mockService.Object, mockSession.Object);
            var result = sut.SearchResults("search");

            //assert
            var viewResult = result as ViewResult;

            var model = viewResult.Model as SearchResultsVM;
            model.Count.Should().Be(0);
            model.ShowResults.Should().Be(true);
            mockService.Verify();
        }

        [Fact]
        public void SearchResults_Should_Return_Internal_Error()
        {
            //arrange
            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<bool>())).Throws(new Exception());
            //act
            var sut = new SearchController(mockService.Object, mockSession.Object);
            var result = sut.SearchResults("search");

            //assert
            var statusResult = result as StatusCodeResult;
            statusResult.StatusCode.Should().Be(500);
            mockService.Verify();
        }

        [Fact]
        public void SearchResultsShouldGetCorrectCountResult()
        {
            //arrange
            var expectedrecord = new Models.SearchResult
            {
                Id = "testid",
                Name = "test location",
                Address = "test address",
                PostCode = "12345",
                Town = "test town",
                Region = "test region",
                Category = "test category"
            };
            var expectedResult = new List<Models.SearchResult>();
            expectedResult.Add(expectedrecord);
            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(expectedResult).Verifiable();
            //act
            var sut = new SearchController(mockService.Object, mockSession.Object);
            var result = sut.SearchResults("search", 1);

            //assert
            var viewResult = result as ViewResult;

            var model = viewResult.Model as SearchResultsVM;
            model.ShowResults.Should().Be(true);
            model.Data.Count.Should().Be(1);
            mockService.Verify();
        }

        [Fact]
        public void SearchResultsShouldGetCorrectFacetResult()
        {
            //arrange
            var expectedrecord = new Models.SearchResult
            {
                Id = "testid",
                Name = "test location",
                Address = "test address",
                PostCode = "12345",
                Town = "test town",
                Region = "test region",
                Category = "test category"
            };
            var expectedResult = new List<Models.SearchResult>();
            expectedResult.Add(expectedrecord);

            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(expectedResult).Verifiable();
            mockService.Setup(x => x.GetFacets()).Returns(new EditableList<string> {"Test Facet"});

            //act
            var sut = new SearchController(mockService.Object, mockSession.Object);
            var result = sut.SearchResults("search", 1, null, "TestFacet");
            
            //assert
            var viewResult = result as ViewResult;

            var model = viewResult.Model as SearchResultsVM;
            model.ShowResults.Should().Be(true);
            model.Facets.Count.Should().Be(1);
            mockService.Verify();
        }

        [Fact]
        public void SearchResultsShouldGetCorrectSelectedFacetResult()
        {
            //arrange
            var search = "test search";
            var expectedrecord = new Models.SearchResult
            {
                Id = "testid",
                Name = "test location",
                Address = "test address",
                PostCode = "12345",
                Town = "test town",
                Region = "test region",
                Category = "test category"
            };
            var expectedResult = new List<Models.SearchResult>();
            expectedResult.Add(expectedrecord);

            var mockSession = new Mock<ISessionService>();
            mockSession.Setup(x => x.GetUserSearch()).Returns(search);
            var mockService = new Mock<ISearchService>();
            mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(expectedResult).Verifiable();
            mockService.Setup(x => x.GetFacets()).Returns(new EditableList<string> { "TestFacet" });

            //act
            var sut = new SearchController(mockService.Object, mockSession.Object);
            var result = sut.SearchResults(search, 1, null, "TestFacet");
            
            //assert
            var viewResult = result as ViewResult;

            var model = viewResult.Model as SearchResultsVM;
            model.ShowResults.Should().Be(true);
            model.Facets[0].Selected.Should().Be(true);
            mockService.Verify();
        }
        */
    }
}
