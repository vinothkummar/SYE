using System;
using System.Collections.Generic;
using System.Linq;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SYE.Helpers.Enums;
using SYE.Models;
using SYE.Services;
using SYE.ViewModels;

namespace SYE.Controllers
{
    public class FormController : BaseController
    {
        private readonly IGdsValidation _gdsValidate;
        private readonly ISessionService _sessionService;
        private readonly IOptions<ApplicationSettings> _config;

        public FormController(IGdsValidation gdsValidate, ISessionService sessionService, IOptions<ApplicationSettings> config, ILogger<FormController> logger)
        {
            _gdsValidate = gdsValidate;
            _sessionService = sessionService;
            _config = config;
        }

        [HttpGet("form/{id}")]
        public IActionResult Index(string id = "")
        {
            try
            {
                var userSession = _sessionService.GetUserSession();
                if (userSession == null)
                {
                    return GetCustomErrorCode(EnumStatusCode.FormPageLoadSessionNullError, "Error with user session. Session is null: Id='" + id + "'");
                }

                if (userSession.LocationName == null)
                {
                    return GetCustomErrorCode(EnumStatusCode.FormPageLoadLocationNullError, "Error with user session. Location is null: Id='" + id + "'");
                }

                var serviceNotFound = userSession.LocationName.Equals("the service");
                PageVM pageVm = null;
                try
                {
                    //if we've got this far then the session is ok
                    //if there an exception then the json cant be found
                    pageVm = _sessionService.GetPageById(id, serviceNotFound);
                }
                catch
                {
                    return GetCustomErrorCode(EnumStatusCode.FormPageLoadJsonError, "Error with json file: Id='" + id + "'");
                }                

                if (pageVm == null)
                {
                    return GetCustomErrorCode(EnumStatusCode.FormPageLoadNullError, "Error with user session. pageVm is null: Id='" + id + "'");
                }

                ViewBag.BackLink = new BackLinkVM { Show = true, Url = GetPreviousPage(pageVm, serviceNotFound), Text = "Back" };

                //Update the users journey
                _sessionService.UpdateNavOrder(pageVm.PageId);

                ViewBag.Title = pageVm.PageTitle + " - Give feedback on care";
                return View(pageVm);
            }
            catch (Exception ex)
            {
                ex.Data.Add("GFCError", "Unexpected error loading form: Id='" + id + "'");
                throw ex;
            }
        }

        private static readonly HashSet<char> allowedChars = new HashSet<char>(@"1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz.,'()?!#&$£%^@*;:+=_-/ ");
        private static readonly List<string> restrictedWords = new List<string> { "javascript", "onblur", "onchange", "onfocus", "onfocusin", "onfocusout", "oninput", "onmouseenter", "onmouseleave",
            "onselect", "onclick", "ondblclick", "onkeydown", "onkeypress", "onkeyup", "onmousedown", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onscroll", "ontouchstart",
            "ontouchend", "ontouchmove", "ontouchcancel", "onwheel" };


        [HttpPost("form/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CurrentPageVM vm)
        {
            try
            {
                PageVM pageVm = null;
                try
                {
                    //Get the current PageVm from Session (throws exception if session is null/timed out)
                    pageVm = _sessionService.GetPageById(vm.PageId, false);
                }
                catch
                {
                    return GetCustomErrorCode(EnumStatusCode.FormPageContinueSessionNullError, "Error with user session. Session is null: Id='" + vm.PageId + "'");
                }

                if (pageVm == null)
                {
                    return GetCustomErrorCode(EnumStatusCode.FormPageContinueNullError, "Error with user session. pageVm is null: Id='" + vm.PageId + "'");
                }
                if (!string.IsNullOrWhiteSpace(_sessionService.PageForEdit))
                {
                    if (_sessionService.PageForEdit == pageVm.PageId)
                    {
                        //this page was revisited and edited
                        _sessionService.RemoveNavOrderFrom(pageVm.PageId);
                    }
                }

                var userSession = _sessionService.GetUserSession();
                if (userSession == null)//shouldn't happen as it's handled above
                {
                    return GetCustomErrorCode(EnumStatusCode.FormPageContinueSessionNullError, "Error with user session. Session is null: Id='" + vm.PageId + "'");
                }
                if (string.IsNullOrWhiteSpace(userSession.LocationName))
                {
                    return GetCustomErrorCode(EnumStatusCode.FormContinueLocationNullError, "Error with user session. Location is null: Id='" + vm.PageId + "'");
                }

                var serviceNotFound = userSession.LocationName.Equals("the service");
                ViewBag.BackLink = new BackLinkVM { Show = true, Url = GetPreviousPage(pageVm, serviceNotFound), Text = "Back" };

                if (Request?.Form != null)
                {
                    //Validate the Response against the page json and update PageVm to contain the answers
                    _gdsValidate.ValidatePage(pageVm, Request.Form, true, restrictedWords);
                }

                //Get the error count
                var errorCount = pageVm.Questions?.Count(m => m.Validation != null && m.Validation.IsErrored);

                //If we have errors return to the View
                if (errorCount > 0) return View(pageVm);

                //Now we need to update the FormVM in session.
                _sessionService.UpdatePageVmInFormVm(pageVm);

                //No errors redirect to the Index page with our new PageId
                var nextPageId = pageVm.NextPageId;


                //************************************************
                //Hack to make Good Journey work
                //Need to refactor into GDS Helpers when we have time
                if (vm.PageId == "give-your-feedback")
                {
                    var formVm = _sessionService.GetFormVmFromSession();
                    var questions = formVm.Pages.SelectMany(m => m.Questions).ToList();

                    var startGoodJourney = questions.FirstOrDefault(m => m.QuestionId == "what-you-want-to-tell-us-about-01");
                    if (startGoodJourney != null && startGoodJourney.Answer == "Good experience")
                    {
                        nextPageId = "can-we-share";
                    }
                }
                //************************************************

                //Check the nextPageId for preset controller names
                switch (nextPageId)
                {
                    case "HowWeUseYourInformation":
                        return RedirectToAction("Index", "HowWeUseYourInformation");

                    case "CheckYourAnswers":
                        return RedirectToAction("Index", "CheckYourAnswers");

                    case "Home":
                        return RedirectToAction("Index", "Home");
                }

                //Finally, No Errors so load the next page
                return RedirectToAction("Index", new { id = nextPageId });

            }
            catch (Exception ex)
            {
                ex.Data.Add("GFCError", "Unexpected error updating PageVM. Id:='" + vm.PageId + "'");
                throw ex;
            }
        }

        private string GetPreviousPage(PageVM currentPage, bool serviceNotFound)
        {
            var form = _sessionService.GetFormVmFromSession();
            var serviceNotFoundPage = _config.Value.ServiceNotFoundPage;
            var startPage = _config.Value.FormStartPage;
            var targetPage = _config.Value.DefaultBackLink;
            var searchUrl = _sessionService.GetSearchUrl();

            //Get all the back options for the current page
            var previousPageOptions = currentPage.PreviousPages?.ToList();

            //Check if we dealing with one of the start pages as they have no back options
            if (previousPageOptions?.Count == 0)
            {
                if (serviceNotFound && currentPage.PageId == serviceNotFoundPage)
                    return Url.Action("Index", "Search");

                if (!serviceNotFound && currentPage.PageId == startPage)
                    return searchUrl;

                if (serviceNotFound && currentPage.PageId == startPage)
                    return Url.Action("Index", "Form", new { id = serviceNotFoundPage });
            }

            //Check if we only have 1 option
            if (previousPageOptions.Count() == 1) return Url.Action("Index", "Form", new { id = previousPageOptions.FirstOrDefault()?.PageId });

            //Get all the questions in the FormVM
            var questions = form.Pages.SelectMany(m => m.Questions).ToList();

            //Loop through each option and return the pageId when 
            foreach (var pageOption in previousPageOptions)
            {
                var answer = questions.FirstOrDefault(m => m.QuestionId == pageOption.QuestionId)?.Answer;
                if (pageOption.Answer == answer)
                    return Url.Action("Index", "Form", new { id = pageOption.PageId });
            }

            return targetPage;
        }

    }
}