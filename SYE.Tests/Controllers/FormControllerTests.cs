using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Moq;
using SYE.Controllers;
using SYE.Models;
using SYE.Services;
using SYE.ViewModels;
using Xunit;
using Microsoft.AspNetCore.Http;
using SYE.Models.Response;

namespace SYE.Tests.Controllers
{
    /// <summary>
    /// this test class is to ensure that the controller is talking to the service layer correctly.
    /// 
    /// </summary>    
    public class FormControllerTests
    {
        [Fact]
        public void Index_Should_Return_Data()
        {
            const string id = "123";
            //arrange
            var returnPage = new PageVM { PageId = id, PreviousPages = new List<PreviousPageVM>() };
            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockUrlHelper = new Mock<IUrlHelper>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(id, false)).Returns(returnPage).Verifiable();
            mockSession.Setup(x => x.GetUserSession()).Returns(new UserSessionVM { LocationName = "" }).Verifiable();
            mockSession.Setup(x => x.GetFormVmFromSession()).Returns(new FormVM());

            ApplicationSettings appSettings = new ApplicationSettings() { FormStartPage = "123" };
            IOptions<ApplicationSettings> options = Options.Create(appSettings);

            var sut = new FormController(mockValidation.Object, mockSession.Object, options, mockLogger.Object);
            sut.Url = mockUrlHelper.Object;
            //act
            var result = sut.Index(id);

            //assert
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as PageVM;
            model.PageId.Should().Be(id);
            mockSession.Verify();
        }

        [Fact]
        public void Index_Should_Return_561_Error()
        {
            const string id = "123";
            //arrange
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetUserSession()).Returns(new UserSessionVM { LocationName = null }).Verifiable();

            ApplicationSettings appSettings = new ApplicationSettings() { FormStartPage = "123" };
            IOptions<ApplicationSettings> options = Options.Create(appSettings);

            var sut = new FormController(mockValidation.Object, mockSession.Object, options, mockLogger.Object){ControllerContext = controllerContext};

            //act
            var result = sut.Index(id);

            //assert
            var statusResult = result as StatusResult;
            statusResult.StatusCode.Should().Be(561);
            mockSession.Verify();
        }
        [Fact]
        public void Index_Should_Return_562_Error()
        {
            const string id = "123";
            //arrange
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            PageVM returnPage = null;
            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(id, false)).Returns(returnPage).Verifiable();
            mockSession.Setup(x => x.GetUserSession()).Returns(new UserSessionVM { LocationName = "" }).Verifiable();

            var mockSettings = new Mock<IOptions<ApplicationSettings>>();
            var sut = new FormController(mockValidation.Object, mockSession.Object, mockSettings.Object, mockLogger.Object) { ControllerContext = controllerContext };

            //act
            var result = sut.Index(id);

            //assert
            var statusResult = result as StatusResult;
            statusResult.StatusCode.Should().Be(562);
            mockSession.Verify();
        }

        [Fact]
        public void Index_Should_Return_563_Error()
        {
            const string id = "123";
            //arrange
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(id, false)).Throws(new Exception());

            var mockSettings = new Mock<IOptions<ApplicationSettings>>();
            var sut = new FormController(mockValidation.Object, mockSession.Object, mockSettings.Object, mockLogger.Object) { ControllerContext = controllerContext };

            //act
            var result = sut.Index(new CurrentPageVM { PageId = id });

            //assert
            var statusResult = result as StatusResult;
            statusResult.StatusCode.Should().Be(563);
            mockSession.Verify();
        }

        [Fact]
        public void Index_Should_Return_565_Error()
        {
            const string id = "123";
            //arrange
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            PageVM returnPage = new PageVM();
            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(It.IsAny<string>(), It.IsAny<bool>())).Returns(returnPage).Verifiable();
            mockSession.Setup(x => x.GetUserSession()).Returns(new UserSessionVM { LocationName = "" }).Verifiable();

            var mockSettings = new Mock<IOptions<ApplicationSettings>>();
            var sut = new FormController(mockValidation.Object, mockSession.Object, mockSettings.Object, mockLogger.Object) { ControllerContext = controllerContext };

            //act
            var result = sut.Index(new CurrentPageVM { PageId = id });

            //assert
            var statusResult = result as StatusResult;
            statusResult.StatusCode.Should().Be(565);
            mockSession.Verify();
        }

        [Fact]
        public void Index_Should_Return_566_Error()
        {
            const string id = "123";
            //arrange
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            PageVM returnPage = new PageVM();
            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(id, false)).Throws(new Exception());
            mockSession.Setup(x => x.GetUserSession()).Returns(new UserSessionVM { LocationName = "" }).Verifiable();

            var mockSettings = new Mock<IOptions<ApplicationSettings>>();
            var sut = new FormController(mockValidation.Object, mockSession.Object, mockSettings.Object, mockLogger.Object) { ControllerContext = controllerContext };

            //act
            var result = sut.Index(id);

            //assert
            var statusResult = result as StatusResult;
            statusResult.StatusCode.Should().Be(566);
            mockSession.Verify();
        }
        [Fact]
        public void Index_Should_Return_Internal_Error()
        {
            const string id = "123";
            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(id, false)).Throws(new Exception()).Verifiable();
            mockSession.Setup(x => x.GetUserSession()).Returns(new UserSessionVM { LocationName = "" }).Verifiable();

            var mockSettings = new Mock<IOptions<ApplicationSettings>>();
            var sut = new FormController(mockValidation.Object, mockSession.Object, mockSettings.Object, mockLogger.Object);
            // Act
            Action action = () => sut.Index(id);
            // Assert
            action.Should().Throw<Exception>().Where(x => x.Data["GFCError"].ToString() == "Unexpected error loading form: Id='" + id + "'");
            mockSession.Verify();
        }

        [Fact]
        public void Index_Post_Should_Return_Not_Found()
        {
            //arrange
            const string id = "123";
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };
            PageVM returnPage = null;
            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(id, false)).Returns(returnPage).Verifiable();
            var mockSettings = new Mock<IOptions<ApplicationSettings>>();
            var sut = new FormController(mockValidation.Object, mockSession.Object, mockSettings.Object, mockLogger.Object){ControllerContext = controllerContext};
            //act
            var result = sut.Index(new CurrentPageVM { PageId = id });
            //assert
            var statusResult = result as StatusResult;
            statusResult.StatusCode.Should().Be(564);
            mockSession.Verify();
        }

        [Fact]
        public void Index_Post_Should_Return_Errors()
        {
            const string id = "123";
            //arrange

            var returnPage = new PageVM { PageId = id, PreviousPages = new List<PreviousPageVM>() };
            var questions = new List<QuestionVM>
            {
                new QuestionVM {Validation = new ValidationVM {IsErrored = true, ErrorMessage = "blah blah"}},
                new QuestionVM {Validation = new ValidationVM {IsErrored = true, ErrorMessage = "blah blah"}}
            };
            returnPage.Questions = questions;

            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(id, false)).Returns(returnPage).Verifiable();
            mockSession.Setup(x => x.GetUserSession()).Returns(new UserSessionVM { LocationName = "the service" }).Verifiable();

            ApplicationSettings appSettings = new ApplicationSettings() { FormStartPage = id, ServiceNotFoundPage = "test1", DefaultBackLink = "test2" };
            IOptions<ApplicationSettings> options = Options.Create(appSettings);

            var mockUrlHelper = new Mock<IUrlHelper>();
            var sut = new FormController(mockValidation.Object, mockSession.Object, options, mockLogger.Object);
            sut.Url = mockUrlHelper.Object;
            //act
            var result = sut.Index(new CurrentPageVM { PageId = id });

            //assert
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as PageVM;
            model.PageId.Should().Be(id);
            model.Questions.Count(x => x.Validation.IsErrored).Should().Be(2);
            mockSession.Verify();
        }

        [Theory]
        [InlineData("CheckYourAnswers")]
        [InlineData(null)]
        public void Index_Post_Should_Redirect(string controllerName)
        {
            const string id = "123";
            //arrange

            var returnPage = new PageVM { PageId = id, NextPageId = controllerName, PreviousPages = new List<PreviousPageVM>() };

            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(id, false)).Returns(returnPage).Verifiable();
            mockSession.Setup(x => x.GetUserSession()).Returns(new UserSessionVM { LocationName = "the service" }).Verifiable();

            ApplicationSettings appSettings = new ApplicationSettings() { FormStartPage = id, ServiceNotFoundPage = "test1", DefaultBackLink = "test2" };
            IOptions<ApplicationSettings> options = Options.Create(appSettings);

            var mockUrlHelper = new Mock<IUrlHelper>();
            var sut = new FormController(mockValidation.Object, mockSession.Object, options, mockLogger.Object);
            sut.Url = mockUrlHelper.Object;

            //act
            var result = sut.Index(new CurrentPageVM { PageId = id });

            //assert
            var redirectesult = result as RedirectToActionResult;
            redirectesult.ControllerName.Should().Be(controllerName);
            redirectesult.ActionName.Should().Be("Index");
            mockSession.Verify();
        }

    }
}
