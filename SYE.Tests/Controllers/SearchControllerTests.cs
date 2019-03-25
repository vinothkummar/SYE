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
        //[Fact]
        //public void Index_Should_Return_Internal_Error()
        //{
        //    //arrange
        //    List<SearchResult> expectedResult = null;
        //    var mockService = new Mock<ISearchService>();
        //    mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
        //        .ReturnsAsync(expectedResult).Verifiable();

        //    //act
        //    var sut = new SearchController(mockService.Object);

        //    var result = sut.GetPaginateResult("search", 1);
        //    //act
        //    var result = sut.Index(id);

        //    //assert
        //    var statusResult = result as StatusCodeResult;
        //    statusResult.StatusCode.Should().Be(500);
        //    mockPageService.Verify();
        //}

        [Fact]
        public void GetPaginateResultShouldGetResult()
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
            var mockService = new Mock<ISearchService>();
            mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expectedResult).Verifiable();

            var sut = new SearchController(mockService.Object);

            var result = sut.GetPaginateResult("search", 1);
            //assert
            var viewResult = result as ViewResult;
           
            var model = viewResult.Model as SearchResultsViewModel;
            model.Data.Count.Should().Be(1);
            mockService.Verify();
        }
        [Fact]
        public void GetPaginateResultShouldGetCorrectResult()
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
            var mockService = new Mock<ISearchService>();
            mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expectedResult).Verifiable();

            var sut = new SearchController(mockService.Object);

            var result = sut.GetPaginateResult("search", 1);
            //assert
            var viewResult = result as ViewResult;

            var model = viewResult.Model as SearchResultsViewModel;
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
        public void GetPaginateResultShouldReturnEmptyList()
        {
            //arrange
            List<SearchResult> expectedResult = null;
            var mockService = new Mock<ISearchService>();
            mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(expectedResult).Verifiable();

            //act
            var sut = new SearchController(mockService.Object);

            var result = sut.GetPaginateResult("search", 1);
            //assert
            var viewResult = result as ViewResult;

            var model = viewResult.Model as SearchResultsViewModel;
            model.Count.Should().Be(0);
            mockService.Verify();
        }

        [Fact]
        public void GetPaginateResultShouldReturnInternalError()
        {
            //arrange
            var mockService = new Mock<ISearchService>();
            mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Throws(new Exception());

            var sut = new SearchController(mockService.Object);

            var result = sut.GetPaginateResult("search", 1);

            //assert
            var statusResult = result as StatusCodeResult;
            statusResult.StatusCode.Should().Be(500);
            mockService.Verify();
        }

    }
}
