using System;
using System.Linq;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using SYE.Models;
using SYE.Services;
using SYE.ViewModels;

namespace SYE.Controllers
{
    public class FormController : Controller
    {
        private readonly IGdsValidation _gdsValidate;
        private readonly ISessionService _sessionService;
        private readonly IOptions<ApplicationSettings> _config;
        private readonly ILogger _logger;

        public FormController(IGdsValidation gdsValidate, ISessionService sessionService, IOptions<ApplicationSettings> config, ILogger<FormController> logger)
        {
            _gdsValidate = gdsValidate;
            _sessionService = sessionService;
            _config = config;
            _logger = logger;
        }

        [HttpGet("form/{id}")]
        public IActionResult Index(string id = "")
        {
            try
            {
                var userSession = _sessionService.GetUserSession();
                var serviceNotFound = userSession.LocationName.Equals("the service");

                var pageVm = _sessionService.GetPageById(id, serviceNotFound);

                if (pageVm == null) return NotFound();

                ViewBag.PreviousPage = GetPreviousPage(pageVm, serviceNotFound);

                //Update the users journey
                _sessionService.UpdateNavOrder(pageVm.PageId);

                return View(pageVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading PageVM.");
                return StatusCode(500);
            }
        }


        [HttpPost("form/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CurrentPageVM vm)
        {
            try
            {
                //Get the current PageVm from Session
                var pageVm = _sessionService.GetPageById(vm.PageId, false);

                //If Null throw NotFound error
                if (pageVm == null) return NotFound();


                var userSession = _sessionService.GetUserSession();
                var serviceNotFound = userSession.LocationName.Equals("the service");
                ViewBag.PreviousPage = GetPreviousPage(pageVm, serviceNotFound);

                if (Request?.Form != null)
                {
                    //Validate the Response against the page json and update PageVm to contain the answers
                    _gdsValidate.ValidatePage(pageVm, Request.Form);
                }

                //Get the error count
                var errorCount = pageVm.Questions?.Count(m => m.Validation != null && m.Validation.IsErrored);

                //If we have errors return to the View
                if (errorCount > 0) return View(pageVm);

                //Now we need to update the FormVM in session.
                _sessionService.UpdatePageVmInFormVm(pageVm);

                //No errors redirect to the Index page with our new PageId
                var nextPageId = pageVm.NextPageId;

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
                _logger.LogError(ex, "Error updating PageVM.");
                return StatusCode(500);
            }
        }




        private string GetPreviousPage(PageVM currentPage, bool serviceNotFound)
        {
            var form = _sessionService.GetFormVmFromSession();
            var serviceNotFoundPage = _config.Value.ServiceNotFoundPage;
            var startPage = _config.Value.FormStartPage;
            var targetPage = _config.Value.DefaultBackLink;

            //Get all the back options for the current page
            var previousPageOptions = currentPage.PreviousPages?.ToList();

            //Check if we dealing with one of the start pages as they have no back options
            if (previousPageOptions?.Count == 0)
            {
                if (serviceNotFound && currentPage.PageId == serviceNotFoundPage)
                    return Url.Action("Index", "Search");

                if (!serviceNotFound && currentPage.PageId == startPage)
                    return Url.Action("Index", "Search");

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