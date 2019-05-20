using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SYE.Controllers;
using SYE.Models;
using SYE.Services;
using Xunit;

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
            var returnPage = new PageVM { PageId = id };
            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(id, false)).Returns(returnPage).Verifiable();
            var sut = new FormController(mockValidation.Object, mockSession.Object, mockLogger.Object);

            //act
            var result = sut.Index(id);

            //assert
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as PageVM;
            model.PageId.Should().Be(id);
            mockSession.Verify();
        }

        [Fact]
        public void Index_Should_Return_Not_Found()
        {
            const string id = "123";
            //arrange
            PageVM returnPage = null;
            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(id, false)).Returns(returnPage).Verifiable();
            var sut = new FormController(mockValidation.Object, mockSession.Object, mockLogger.Object);

            //act
            var result = sut.Index(id);

            //assert
            var statusResult = result as StatusCodeResult;
            statusResult.StatusCode.Should().Be(404);
            mockSession.Verify();
        }

        [Fact]
        public void Index_Should_Return_Internal_Error()
        {
            const string id = "123";
            //arrange
            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(id, false)).Throws(new Exception()).Verifiable();
            var sut = new FormController(mockValidation.Object, mockSession.Object, mockLogger.Object);

            //act
            var result = sut.Index(id);

            //assert
            var statusResult = result as StatusCodeResult;
            statusResult.StatusCode.Should().Be(500);
            mockSession.Verify();
        }

        [Fact]
        public void Index_Post_Should_Return_Not_Found()
        {
            const string id = "123";
            //arrange
            PageVM returnPage = null;
            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(id, false)).Returns(returnPage).Verifiable();
            var sut = new FormController(mockValidation.Object, mockSession.Object, mockLogger.Object);

            //act
            var result = sut.Index(new CurrentPageVM{PageId = id});

            //assert
            var statusResult = result as StatusCodeResult;
            statusResult.StatusCode.Should().Be(404);
            mockSession.Verify();
        }

        [Fact]
        public void Index_Post_Should_Return_Errors()
        {
            const string id = "123";
            //arrange

            var returnPage = new PageVM{PageId = id};
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
            var sut = new FormController(mockValidation.Object, mockSession.Object, mockLogger.Object);

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

            var returnPage = new PageVM { PageId = id, NextPageId = controllerName };

            var mockValidation = new Mock<IGdsValidation>();
            var mockSession = new Mock<ISessionService>();
            var mockLogger = new Mock<ILogger<FormController>>();
            mockSession.Setup(x => x.GetPageById(id, false)).Returns(returnPage).Verifiable();
            var sut = new FormController(mockValidation.Object, mockSession.Object, mockLogger.Object);

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
