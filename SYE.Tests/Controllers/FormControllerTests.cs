using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Moq;
using SYE.Controllers;
using SYE.Services;
using Xunit;

namespace SYE.Tests.Controllers
{
    public class FormControllerTests
    {
        [Fact]
        public void Index_Should_Return_Data()
        {
            const string id = "123";
            //arrange
            var returnPage = new PageVM {PageId = id};
            var mockValidation = new Mock<IGdsValidation>();
            var mockPageService = new Mock<IPageService>();
            mockPageService.Setup(x => x.GetPageById(id, It.IsAny<string>(), It.IsAny<string>())).Returns(returnPage);
            var sut = new FormController(mockValidation.Object, mockPageService.Object);

            //act
            var result = sut.Index(id);

            //assert
            var viewResult = result as ViewResult;
            var model = viewResult.ViewData.Model as PageVM;
            model.PageId.Should().Be(id);
        }
        [Fact]
        public void Index_Should_Return_Not_Found()
        {
            const string id = "123";
            //arrange
            PageVM returnPage = null;
            var mockValidation = new Mock<IGdsValidation>();
            var mockPageService = new Mock<IPageService>();
            mockPageService.Setup(x => x.GetPageById(id, It.IsAny<string>(), It.IsAny<string>())).Returns(returnPage);
            var sut = new FormController(mockValidation.Object, mockPageService.Object);

            //act
            var result = sut.Index(id);

            //assert
            var statusResult = result as StatusCodeResult;
            statusResult.StatusCode.Should().Be(404);
        }
    }
    #region CodeToMockHttpContext
    //Mock mockHttpContext = new Mock(); Mock mockSession = new Mock().As();

    //Mock<HttpContext> mockHttpContext = new Mock<HttpContext>();
    //Mock<ISession> mockSession = new Mock<ISession>();
    //mockSession["Key"] = "123";
    //mockHttpContext.Setup(s => s.Session).Returns(mockSession.Object);

    //var mockHttpContext = new Mock<DefaultHttpContext>();
    //var mockSession = new Mock<ISession>();
    //IEnumerable<string> sessionKeys = new string[] { };
    ////Convert to list.
    //List<string> listSessionKeys = sessionKeys.ToList();
    //listSessionKeys.Add("ModuleId");
    //sessionKeys = listSessionKeys;
    //mockSession.Setup(s => s.Keys).Returns(sessionKeys);
    //mockSession.Setup(s => s.Id).Returns("89eca97a-872a-4ba2-06fe-ba715c3f32be");
    //mockSession.Setup(s => s.IsAvailable).Returns(true);
    //mockHttpContext.Setup(s => s.Session).Returns(mockSession.Object);
    //mockSession.Setup(s => s.GetString("ModuleId")).Returns("1");
    //sut.ControllerContext = new ControllerContext();
    //sut.ControllerContext.HttpContext = new DefaultHttpContext();
    //sut.ControllerContext.HttpContext.Request.Headers["device-id"] = "20317";
    //mockHttpContext.Setup(s => s.Session).Returns(mockSession.Object);
    //mockSession.Setup(s => s.GetString("LocationName")).Returns("1");

    //mockSession.Setup(x => x.GetString("LocationName")).Returns("The Thatched House Dental Practise");
    //sut.ControllerContext.HttpContext.Session = mockSession.Object;

    #endregion
}
