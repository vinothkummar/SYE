using System;
using FluentAssertions;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SYE.Controllers;
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
        [Fact(Skip = "Update tests to include data access.")]
        public void Index_Should_Return_Data()
        {
            const string id = "123";
            //arrange
            var returnPage = new PageVM {PageId = id};
            var mockValidation = new Mock<IGdsValidation>();
            var mockPageService = new Mock<IPageService>();
            var mockSessionService = new Mock<ISessionService>();
            mockPageService.Setup(x => x.GetPageById(id, It.IsAny<string>(), It.IsAny<string>())).Returns(returnPage).Verifiable();
            var sut = new FormController(mockValidation.Object, mockPageService.Object, mockSessionService.Object);

            //act
            var result = sut.Index(id);

            //assert
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as PageVM;
            model.PageId.Should().Be(id);
            mockPageService.Verify();
        }
        [Fact(Skip = "Update tests to include data access.")]
        public void Index_Should_Return_Not_Found()
        {
            const string id = "123";
            //arrange
            PageVM returnPage = null;
            var mockValidation = new Mock<IGdsValidation>();
            var mockPageService = new Mock<IPageService>();
            var mockSessionService = new Mock<ISessionService>();
            mockPageService.Setup(x => x.GetPageById(id, It.IsAny<string>(), It.IsAny<string>())).Returns(returnPage).Verifiable();
            var sut = new FormController(mockValidation.Object, mockPageService.Object, mockSessionService.Object);

            //act
            var result = sut.Index(id);

            //assert
            var statusResult = result as StatusCodeResult;
            statusResult.StatusCode.Should().Be(404);
            mockPageService.Verify();
        }
        [Fact(Skip = "Update tests to include data access.")]
        public void Index_Should_Return_Internal_Error()
        {
            const string id = "123";
            //arrange
            var mockValidation = new Mock<IGdsValidation>();
            var mockPageService = new Mock<IPageService>();
            var mockSessionService = new Mock<ISessionService>();
            mockPageService.Setup(x => x.GetPageById(id, It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception()).Verifiable();
            var sut = new FormController(mockValidation.Object, mockPageService.Object, mockSessionService.Object);

            //act
            var result = sut.Index(id);

            //assert
            var statusResult = result as StatusCodeResult;
            statusResult.StatusCode.Should().Be(500);
            mockPageService.Verify();
        }
    }
}
