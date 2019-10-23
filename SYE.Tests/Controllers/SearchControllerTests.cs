using System;
using System.Collections.Generic;
using FluentAssertions;
using GDSHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;
using Moq;
using SYE.Controllers;
using SYE.Models;
using SYE.Models.Response;
using SYE.Services;
using SYE.ViewModels;
using Xunit;

namespace SYE.Tests.Controllers
{
    public class SearchControllerTests
    {
        //[Fact]
        //public void SearchResultsShouldGetCorrectResult()
        //{
        //    //arrange
        //    var expectedRecord = new Models.SearchResult
        //    {
        //        Id = "testid",
        //        Name = "test location",
        //        Address = "test address",
        //        PostCode = "12345",
        //        Town = "test town",
        //        Region = "test region",
        //        Category = "test category"
        //    };
        //    var expectedResult = new SearchServiceResult() { Data = new List<SearchResult>() };
        //    expectedResult.Data.Add(expectedRecord);
        //    var mockSession = new Mock<ISessionService>();
        //    var mockService = new Mock<ISearchService>();
        //    mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
        //        It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(expectedResult).Verifiable();
        //    var mockSettings = new Mock<IOptions<ApplicationSettings>>();
        //    var mockLogger = new Mock<ILogger<SearchController>>();
        //    var mockValidation = new Mock<IGdsValidation>();
        //    mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");
        //    //act
        //    var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockLogger.Object, mockValidation.Object);
        //    var result = sut.SearchResults("search", null);

        //    //assert
        //    var viewResult = result as ViewResult;

        //    var model = viewResult.Model as SearchResultsVM;
        //    model.ShowResults.Should().Be(true);
        //    var resultToCompare = model.Data[0];
        //    resultToCompare.Id.Should().Be(expectedRecord.Id);
        //    resultToCompare.Name.Should().Be(expectedRecord.Name);
        //    resultToCompare.Address.Should().Be(expectedRecord.Address);
        //    resultToCompare.PostCode.Should().Be(expectedRecord.PostCode);
        //    resultToCompare.Town.Should().Be(expectedRecord.Town);
        //    resultToCompare.Region.Should().Be(expectedRecord.Region);
        //    resultToCompare.Category.Should().Be(expectedRecord.Category);
        //    mockService.Verify();
        //}

        //[Fact]
        //public void SearchResultsShouldReturnEmptyList()
        //{
        //    //arrange
        //    SearchServiceResult expectedResult = null;
        //    var mockSession = new Mock<ISessionService>();
        //    var mockService = new Mock<ISearchService>();
        //    mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
        //        It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(expectedResult).Verifiable();
        //    var mockSettings = new Mock<IOptions<ApplicationSettings>>();
        //    var mockLogger = new Mock<ILogger<SearchController>>();
        //    var mockValidation = new Mock<IGdsValidation>();
        //    mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

        //    //act
        //    var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockLogger.Object, mockValidation.Object);
        //    var result = sut.SearchResults("search", null);

        //    //assert
        //    var viewResult = result as ViewResult;

        //    var model = viewResult.Model as SearchResultsVM;
        //    model.Count.Should().Be(0);
        //    model.ShowResults.Should().Be(true);
        //    mockService.Verify();
        //}

        //[Fact]
        //public void SearchResultsShouldReturnMaxSearchCharsError()
        //{
        //    //arrange

        //    var search = new string('*', 5000);

        //    var mockSession = new Mock<ISessionService>();
        //    var mockService = new Mock<ISearchService>();
        //    var mockSettings = new Mock<IOptions<ApplicationSettings>>();
        //    var mockLogger = new Mock<ILogger<SearchController>>();
        //    var mockValidation = new Mock<IGdsValidation>();
        //    mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

        //    //act
        //    var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockLogger.Object, mockValidation.Object);
        //    var result = sut.SearchResults(search, null);

        //    //assert
        //    var viewResult = result as ViewResult;

        //    var model = viewResult.Model as SearchResultsVM;
        //    model.ShowExceededMaxLengthMessage.Should().Be(true);
        //    model.ShowResults.Should().Be(false);
        //}

        [Fact]
        public void SearchResults_Should_Return_Internal_Error()
        {
            //arrange
            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<string>(), It.IsAny<bool>())).Throws(new Exception());
            var mockSettings = new Mock<IOptions<ApplicationSettings>>();
            var mockValidation = new Mock<IGdsValidation>();
            mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");
            //act
            var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockValidation.Object);
            Action act = () => sut.SearchResults("search",  null);
            //assert
            act.Should().Throw<Exception>().Where(ex => ex.Data.Contains("GFCError"));
            mockService.Verify();
        }

        //[Fact]
        //public void SearchResultsShouldGetCorrectCountResult()
        //{
        //    //arrange
        //    var expectedrecord = new Models.SearchResult
        //    {
        //        Id = "testid",
        //        Name = "test location",
        //        Address = "test address",
        //        PostCode = "12345",
        //        Town = "test town",
        //        Region = "test region",
        //        Category = "test category"
        //    };
        //    var expectedResult = new SearchServiceResult() { Data = new List<Models.SearchResult>() };
        //    expectedResult.Data.Add(expectedrecord);
        //    var mockSession = new Mock<ISessionService>();
        //    var mockService = new Mock<ISearchService>();
        //    mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
        //        It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(expectedResult).Verifiable();
        //    var mockSettings = new Mock<IOptions<ApplicationSettings>>();
        //    var mockLogger = new Mock<ILogger<SearchController>>();
        //    var mockValidation = new Mock<IGdsValidation>();
        //    mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

        //    //act
        //    var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockLogger.Object, mockValidation.Object);
        //    var result = sut.SearchResults("search", 1);

        //    //assert
        //    var viewResult = result as ViewResult;

        //    var model = viewResult.Model as SearchResultsVM;
        //    model.ShowResults.Should().Be(true);
        //    model.Data.Count.Should().Be(1);
        //    mockService.Verify();
        //}

        //[Fact]
        //public void SearchResultsShouldGetCorrectFacetResult()
        //{
        //    //arrange
        //    var expectedrecord = new Models.SearchResult
        //    {
        //        Id = "testid",
        //        Name = "test location",
        //        Address = "test address",
        //        PostCode = "12345",
        //        Town = "test town",
        //        Region = "test region",
        //        Category = "test category"
        //    };
        //    var expectedResult = new SearchServiceResult()
        //    {
        //        Facets = new EditableList<string> { "Test Facet" },
        //        Data = new List<Models.SearchResult>()
        //    };
        //    expectedResult.Data.Add(expectedrecord);

        //    var mockSession = new Mock<ISessionService>();
        //    var mockService = new Mock<ISearchService>();
        //    mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
        //        It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(expectedResult).Verifiable();
        //    //mockService.Setup(x => x.GetFacets()).Returns(new EditableList<string> { "Test Facet" });
        //    var mockSettings = new Mock<IOptions<ApplicationSettings>>();
        //    var mockLogger = new Mock<ILogger<SearchController>>();
        //    var mockValidation = new Mock<IGdsValidation>();
        //    mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

        //    //act
        //    var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockLogger.Object, mockValidation.Object);
        //    var result = sut.SearchResults("search", 1, "TestFacet");

        //    //assert
        //    var viewResult = result as ViewResult;

        //    var model = viewResult.Model as SearchResultsVM;
        //    model.ShowResults.Should().Be(true);
        //    model.Facets.Count.Should().Be(1);
        //    mockService.Verify();
        //}

        //[Fact]
        //public void SearchResultsShouldGetCorrectSelectedFacetResult()
        //{
        //    //arrange
        //    var search = "test search";
        //    var expectedrecord = new Models.SearchResult
        //    {
        //        Id = "testid",
        //        Name = "test location",
        //        Address = "test address",
        //        PostCode = "12345",
        //        Town = "test town",
        //        Region = "test region",
        //        Category = "test category"
        //    };
        //    var expectedResult = new SearchServiceResult()
        //    {
        //        Facets = new EditableList<string> { "TestFacet" },
        //        Data = new List<Models.SearchResult>()
        //    };
        //    expectedResult.Data.Add(expectedrecord);

        //    var mockSession = new Mock<ISessionService>();
        //    mockSession.Setup(x => x.GetUserSearch()).Returns(search);
        //    var mockService = new Mock<ISearchService>();
        //    mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
        //        It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(expectedResult).Verifiable();
        //    //mockService.Setup(x => x.GetFacets()).Returns(new EditableList<string> { "TestFacet" });
        //    var mockSettings = new Mock<IOptions<ApplicationSettings>>();
        //    var mockLogger = new Mock<ILogger<SearchController>>();
        //    var mockValidation = new Mock<IGdsValidation>();
        //    mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

        //    //act
        //    var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockLogger.Object, mockValidation.Object);
        //    var result = sut.SearchResults(search, 1, "TestFacet");

        //    //assert
        //    var viewResult = result as ViewResult;

        //    var model = viewResult.Model as SearchResultsVM;
        //    model.ShowResults.Should().Be(true);
        //    model.Facets[0].Selected.Should().Be(true);
        //    mockService.Verify();
        //}

        //[Fact]
        //public void SearchResultsShouldApplyCorrectSelectedFacet()
        //{
        //    //arrange
        //    var search = "test search";
        //    var expectedrecord = new Models.SearchResult
        //    {
        //        Id = "testid",
        //        Name = "test location",
        //        Address = "test address",
        //        PostCode = "12345",
        //        Town = "test town",
        //        Region = "test region",
        //        Category = "test category"
        //    };
        //    var expectedResult = new SearchServiceResult() { Data = new List<SearchResult> { expectedrecord } };

        //    var facets = new List<SelectItem>
        //    {
        //        new SelectItem {Text = "Facet1", Selected = true},
        //        new SelectItem {Text = "Facet2", Selected = true},
        //        new SelectItem {Text = "Facet3", Selected = false},
        //        new SelectItem {Text = "Face41", Selected = false}
        //    };
        //    var expectedTotalCount = facets.Count();
        //    var expectedSelectedCount = facets.Count(x => x.Selected);

        //    var facetsList = new EditableList<string>();
        //    facetsList.AddRange(facets.Select(x => x.Text));
        //    expectedResult.Facets = facetsList;

        //    var mockSession = new Mock<ISessionService>();
        //    mockSession.Setup(x => x.GetUserSearch()).Returns(search);
        //    var mockService = new Mock<ISearchService>();
        //    mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(),
        //        It.IsAny<string>(), It.IsAny<bool>())).ReturnsAsync(expectedResult).Verifiable();
        //    //mockService.Setup(x => x.GetFacets()).Returns(facetsList);
        //    var mockSettings = new Mock<IOptions<ApplicationSettings>>();
        //    var mockLogger = new Mock<ILogger<SearchController>>();
        //    var mockValidation = new Mock<IGdsValidation>();
        //    mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

        //    //act
        //    var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockLogger.Object, mockValidation.Object);
        //    var result = sut.SearchResults(search, facets);

        //    //assert
        //    var viewResult = result as ViewResult;

        //    var model = viewResult.Model as SearchResultsVM;
        //    model.ShowResults.Should().Be(true);

        //    //check number of facets is correct
        //    model.Facets.Count.Should().Be(expectedTotalCount);
        //    model.Facets.Count(x => x.Selected).Should().Be(expectedSelectedCount);

        //    //check the selected facets are correct
        //    var selected = model.Facets.Where(x => x.Selected).Select(x => x.Text).ToList();
        //    foreach (var facet in facets.Where(x => x.Selected))
        //    {
        //        selected.Contains(facet.Text).Should().BeTrue();
        //    }

        //    mockService.Verify();
        //}
        
        //[Theory]
        //[InlineData("")]
        //[InlineData(null)]
        //public void SearchResultsWithEmptySearchShouldRedirectToIndex(string search)
        //{
        //    //arrange
        //    var mockSession = new Mock<ISessionService>();
        //    var mockService = new Mock<ISearchService>();
        //    var mockSettings = new Mock<IOptions<ApplicationSettings>>();
        //    var mockLogger = new Mock<ILogger<SearchController>>();
        //    var mockValidation = new Mock<IGdsValidation>();
        //    mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

        //    //act
        //    var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockLogger.Object, mockValidation.Object);
        //    var result = sut.SearchResults(search, 1);

        //    //assert
        //    var redirectResult = result as RedirectToActionResult;

        //    redirectResult.ActionName.Should().Be("Index");
        //}

        //[Theory]
        //[InlineData("")]
        //[InlineData(null)]
        //public void SearchResultsWithEmptySearchShouldRedirectToIndexWithErrorFlag(string search)
        //{
        //    //arrange
        //    var mockSession = new Mock<ISessionService>();
        //    var mockService = new Mock<ISearchService>();
        //    var mockSettings = new Mock<IOptions<ApplicationSettings>>();
        //    var mockLogger = new Mock<ILogger<SearchController>>();
        //    var mockValidation = new Mock<IGdsValidation>();
        //    mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

        //    //act
        //    var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockLogger.Object, mockValidation.Object);
        //    var result = sut.SearchResults(search, 1);

        //    //assert
        //    var redirectResult = result as RedirectToActionResult;
        //    object val;
        //    redirectResult.RouteValues.TryGetValue("isError", out val).Should().Be(true);
        //    val.Should().Be(true);
        //}

        [Fact]
        public void SearchResultsShouldReturnInternalError()
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
                It.IsAny<string>(), It.IsAny<bool>())).Throws(new Exception());
            var mockSettings = new Mock<IOptions<ApplicationSettings>>();
            var mockValidation = new Mock<IGdsValidation>();
            mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

            //act
            var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockValidation.Object);
            Action act = () => sut.SearchResults("search", 1);

            //assert
            act.Should().Throw<Exception>().Where(ex => ex.Data.Contains("GFCError"));
        }

        [Fact]
        public void IndexShouldReturnErrorFlag()
        {
            //arrange
            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            var mockSettings = new Mock<IOptions<ApplicationSettings>>();
            var mockUrlHelper = new Mock<IUrlHelper>();
            var mockValidation = new Mock<IGdsValidation>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockTempDataProvider = new Mock<SessionStateTempDataProvider>();
            mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

            var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockValidation.Object);

            sut.TempData = new TempDataDictionary(mockHttpContext.Object, mockTempDataProvider.Object);
            sut.Url = mockUrlHelper.Object;

            //act
            var result = sut.Index("error message", null, false);

            //assert
            var viewResult = result as ViewResult;

            var model = viewResult.Model as SearchResultsVM;
            model.ShowResults.Should().Be(false);
            model.ShowIncompletedSearchMessage.Should().Be(true);
        }

        //[Fact(Skip = "can't test index exception as it doesn't do anything yet")]
        //public void IndexShouldReturnInternalError()
        //{
        //    //arrange
        //    var mockSession = new Mock<ISessionService>();
        //    var mockService = new Mock<ISearchService>();
        //    var mockSettings = new Mock<IOptions<ApplicationSettings>>();
        //    var mockLogger = new Mock<ILogger<SearchController>>();
        //    var mockValidation = new Mock<IGdsValidation>();
        //    mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

        //    //act
        //    var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockLogger.Object, mockValidation.Object);
        //    var result = sut.Index(true);

        //    //assert
        //    var statusResult = result as StatusCodeResult;
        //    statusResult.StatusCode.Should().Be(500);
        //}

        [Fact]
        public void SelectLocationShouldRedirectToFormIndex()
        {
            //arrange
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            ApplicationSettings appSettings = new ApplicationSettings() { FormStartPage = "test" };
            IOptions<ApplicationSettings> options = Options.Create(appSettings);

            var mockValidation = new Mock<IGdsValidation>();
            mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

            //act
            var sut = new SearchController(mockService.Object, mockSession.Object, options, mockValidation.Object);
            sut.ControllerContext = controllerContext;
            var result = sut.SelectLocation(new UserSessionVM());

            //assert
            var redirectResult = result as RedirectToActionResult;

            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Form");
        }

        [Fact]
        public void SelectLocationShouldCallSessionToSaveProvider()
        {
            //arrange
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var userVm = new UserSessionVM { ProviderId = "123", LocationId = "234", LocationName = "test location" };
            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            mockSession.Setup(x => x.SetUserSessionVars(userVm)).Verifiable();
            ApplicationSettings appSettings = new ApplicationSettings() { FormStartPage = "123" };
            IOptions<ApplicationSettings> options = Options.Create(appSettings);
            var mockValidation = new Mock<IGdsValidation>();
            mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

            //act
            var sut = new SearchController(mockService.Object, mockSession.Object, options, mockValidation.Object);
            sut.ControllerContext = controllerContext;
            sut.SelectLocation(userVm);

            //assert
            mockSession.Verify();
        }

        [Fact]
        public void SelectLocationShouldCallSessionToSaveForm()
        {
            //arrange
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var locationName = "test location";
            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            var replacements = new Dictionary<string, string>
            {
                {"!!location_name!!", locationName}
            };
            mockSession.Setup(x => x.LoadLatestFormIntoSession(replacements)).Verifiable();
            ApplicationSettings appSettings = new ApplicationSettings() { FormStartPage = "123" };
            IOptions<ApplicationSettings> options = Options.Create(appSettings);
            var mockValidation = new Mock<IGdsValidation>();
            mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");

            //act
            var sut = new SearchController(mockService.Object, mockSession.Object, options, mockValidation.Object);
            sut.ControllerContext = controllerContext;
            sut.SelectLocation(new UserSessionVM { LocationName = locationName });

            //assert
            mockSession.Verify();
        }

        [Fact]
        public void SelectLocationShouldReturnInternalError()
        {
            //arrange
            var userVm = new UserSessionVM { ProviderId = "123", LocationId = "234", LocationName = "test location" };
            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            mockSession.Setup(x => x.SetUserSessionVars(userVm)).Throws(new Exception());
            var mockSettings = new Mock<IOptions<ApplicationSettings>>();
            var mockValidation = new Mock<IGdsValidation>();
            mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");
            //act
            var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockValidation.Object);
            Action act = () => sut.SelectLocation(userVm);
            //assert
            act.Should().Throw<Exception>().Where(ex => ex.Data.Contains("GFCError"));
        }
        [Fact]
        public void SearchShouldReturn550StatusCode()
        {
            //arrange
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var userVm = new UserSessionVM { ProviderId = "123", LocationId = "234", LocationName = "test location" };
            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            mockService.Setup(x => x.GetPaginatedResult(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), true)).Throws(new Exception());
            var mockSettings = new Mock<IOptions<ApplicationSettings>>();
            var mockValidation = new Mock<IGdsValidation>();
            mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");
            //act
            var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockValidation.Object);
            sut.ControllerContext = controllerContext;
            var response = sut.SearchResults("searchString", null);
            //assert
            var result = response as StatusResult;
            result.StatusCode.Should().Be(550);
        }
        [Fact]
        public void SelectLocationShouldReturn551ErrorCode()
        {
            //arrange
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var userVm = new UserSessionVM { ProviderId = "123", LocationId = "234", LocationName = "test location" };
            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            mockSession.Setup(x => x.LoadLatestFormIntoSession(It.IsAny<Dictionary<string, string>>())).Throws(new Exception());
            var mockSettings = new Mock<IOptions<ApplicationSettings>>();
            var mockValidation = new Mock<IGdsValidation>();
            mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");
            //act
            var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockValidation.Object);
            sut.ControllerContext = controllerContext;
            var response = sut.SelectLocation(userVm);
            //assert
            var result = response as StatusResult;
            result.StatusCode.Should().Be(551);
        }
        [Fact]
        public void LocationNotFoundShouldReturn552ErrorCode()
        {
            //arrange
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var userVm = new UserSessionVM { ProviderId = "123", LocationId = "234", LocationName = "test location" };
            var mockSession = new Mock<ISessionService>();
            var mockService = new Mock<ISearchService>();
            mockSession.Setup(x => x.LoadLatestFormIntoSession(It.IsAny<Dictionary<string, string>>())).Throws(new Exception());
            var mockSettings = new Mock<IOptions<ApplicationSettings>>();
            var mockValidation = new Mock<IGdsValidation>();
            mockValidation.Setup(x => x.CleanText(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<List<string>>(), It.IsAny<HashSet<char>>())).Returns("abc");
            //act
            var sut = new SearchController(mockService.Object, mockSession.Object, mockSettings.Object, mockValidation.Object);
            sut.ControllerContext = controllerContext;
            var response = sut.LocationNotFound();
            //assert
            var result = response as StatusResult;
            result.StatusCode.Should().Be(552);
        }
    }
}
