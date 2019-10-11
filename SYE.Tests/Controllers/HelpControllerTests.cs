using System;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SYE.Controllers;
using SYE.Models.Response;
using Xunit;

namespace SYE.Tests.Controllers
{
    public class HelpControllerTests
    {

        [Fact]
        public void ReportaProblemShouldReturn555StatusCode()
        {
            //arrange
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var mockService = new Mock<IServiceProvider>();
            //act
            var sut = new HelpController(mockService.Object);
            sut.ControllerContext = controllerContext;
            var response = sut.Feedback("urlReferer");
            //assert
            var result = response as StatusResult;
            result.StatusCode.Should().Be(555);
        }

        [Fact]
        public void ReportaProblemsubmitShouldReturn556StatusCode()
        {
            //arrange
            //Controller needs a controller context
            var httpContext = new DefaultHttpContext();
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var mockService = new Mock<IServiceProvider>();
            //act
            var sut = new HelpController(mockService.Object);
            sut.ControllerContext = controllerContext;
            var response = sut.SubmitFeedback("urlReferer");
            //assert
            var result = response as StatusResult;
            result.StatusCode.Should().Be(556);
        }

    }
}
