using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SYE.Controllers;
using SYE.Models;
using SYE.Services;
using Xunit;

namespace SYE.Tests.Controllers
{
    public class CheckYourAnswersControllerTests
    {
        //[Fact]
        //public void Index_Should_Return_Data()
        //{
        //    const string id = "123";
        //    //arrange
        //    var returnForm = new FormVM{Id = id};
        //    var mockSubmissionService = new Mock<ISubmissionService>();
        //    var mockSession = new Mock<ISessionService>();
        //    mockSession.Setup(x => x.GetFormVmFromSession()).Returns(returnForm).Verifiable();
        //    var sut = new CheckYourAnswersController(mockSession.Object, mockSubmissionService.Object);

        //    //act
        //    var result = sut.Index();

        //    //assert
        //    var viewResult = result as ViewResult;
        //    var model = viewResult.ViewData.Model as CheckYourAnswersVm;
        //    model.FormVm.Id.Should().Be(id);
        //    mockSession.Verify();
        //}

        //[Fact]
        //public void Index_Should_Return_NotFound()
        //{
        //    //arrange
        //    FormVM returnForm = null;
        //    var mockSubmissionService = new Mock<ISubmissionService>();
        //    var mockSession = new Mock<ISessionService>();
        //    mockSession.Setup(x => x.GetFormVmFromSession()).Returns(returnForm).Verifiable();
        //    var sut = new CheckYourAnswersController(mockSession.Object, mockSubmissionService.Object);

        //    //act
        //    var result = sut.Index();

        //    //assert
        //    var statusResult = result as StatusCodeResult;
        //    statusResult.StatusCode.Should().Be(404);
        //    mockSession.Verify();
        //}

        //[Fact]
        //public void Index_Should_Return_Internal_Error()
        //{
        //    var mockSubmissionService = new Mock<ISubmissionService>();
        //    var mockSession = new Mock<ISessionService>();
        //    mockSession.Setup(x => x.GetFormVmFromSession()).Throws(new Exception()).Verifiable();
        //    var sut = new CheckYourAnswersController(mockSession.Object, mockSubmissionService.Object);

        //    //act
        //    var result = sut.Index();

        //    //assert
        //    var statusResult = result as StatusCodeResult;
        //    statusResult.StatusCode.Should().Be(500);
        //    mockSession.Verify();
        //}

        //[Fact(Skip = "Failing. We may need to put GenerateSubmission into the submission service")]
        //public void Index_Post_Should_Return_Data()
        //{
        //    const string id = "123";
        //    //arrange
        //    var returnForm = new FormVM { Id = id };
        //    var mockSubmissionService = new Mock<ISubmissionService>();
        //    var mockSession = new Mock<ISessionService>();
        //    mockSession.Setup(x => x.GetFormVmFromSession()).Returns(returnForm).Verifiable();
        //    var sut = new CheckYourAnswersController(mockSession.Object, mockSubmissionService.Object);

        //    //act
        //    var result = sut.Index(new CheckYourAnswersVm());

        //    //assert
        //    var viewResult = result as ViewResult;
        //    var model = viewResult.ViewData.Model as CheckYourAnswersVm;
        //    model.FormVm.Id.Should().Be(id);
        //    mockSession.Verify();
        //}

        //[Fact]
        //public void Index_Post_Should_Return_Not_Found()
        //{
        //    const string id = "123";
        //    //arrange
        //    FormVM returnForm = null;
        //    var mockSubmissionService = new Mock<ISubmissionService>();
        //    var mockSession = new Mock<ISessionService>();
        //    mockSession.Setup(x => x.GetFormVmFromSession()).Returns(returnForm).Verifiable();
        //    var sut = new CheckYourAnswersController(mockSession.Object, mockSubmissionService.Object);

        //    //act
        //    var result = sut.Index(new CheckYourAnswersVm());

        //    //assert
        //    var statusResult = result as StatusCodeResult;
        //    statusResult.StatusCode.Should().Be(404);
        //    mockSession.Verify();
        //}

        //[Fact]
        //public void Index_Post_Should_Return_Internal_Error()
        //{
        //    const string id = "123";
        //    //arrange
        //    FormVM returnForm = null;
        //    var mockSubmissionService = new Mock<ISubmissionService>();
        //    var mockSession = new Mock<ISessionService>();
        //    mockSession.Setup(x => x.GetFormVmFromSession()).Throws(new Exception()).Verifiable();
        //    var sut = new CheckYourAnswersController(mockSession.Object, mockSubmissionService.Object);

        //    //act
        //    var result = sut.Index(new CheckYourAnswersVm());

        //    //assert
        //    var statusResult = result as StatusCodeResult;
        //    statusResult.StatusCode.Should().Be(500);
        //    mockSession.Verify();
        //}

    }
}
