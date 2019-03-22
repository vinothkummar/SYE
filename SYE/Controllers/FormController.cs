using System;
using System.Linq;
using System.Text;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SYE.Services;

namespace SYE.Controllers
{
    public class FormController : Controller
    {
        private readonly IGdsValidation _gdsValidate;
        private readonly ISessionService _sessionService;

        public FormController(IGdsValidation gdsValidate, ISessionService sessionService)
        {
            _gdsValidate = gdsValidate;
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index(string id = "")
        {
            try
            {
                var pageVm = _sessionService.GetPageById(id);

                if (pageVm != null)
                {
                    return View(pageVm);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                //TODO: log error
                return StatusCode(500);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(PageVM vm)
        {
            try
            {
                var pageVm = _sessionService.GetPageById(vm.PageId);

                if (pageVm == null)
                {
                    return NotFound();
                }

                //Validate the Response against the page json and update PageVm to contain the answers
                _gdsValidate.ValidatePage(pageVm, Request.Form);

                //Get the error count
                var errorCount = pageVm.Questions.Count(m => m.Validation.IsErrored);

                //No errors redirect to the Index page with our new PageId
                var nextPageId = pageVm.NextPageId;

                //If we have errors return to the View
                if (errorCount > 0)
                {
                    return View(pageVm);
                }


                //Now we need to update the FormVM in session.
                var formVm = _sessionService.GetFormVmFromSession();
                var currentPage = formVm.Pages.FirstOrDefault(m => m.PageId == vm.PageId)?.Questions;
                foreach (var question in pageVm.Questions)
                {
                    var q = currentPage.FirstOrDefault(m => m.QuestionId == question.QuestionId);
                    q.Answer = question.Answer;
                }
                _sessionService.SaveFormVmToSession(formVm);
                

                //Check the nextPageId for preset controller names
                if (nextPageId == "CheckYourAnswers")
                {
                    return RedirectToAction("Index", "CheckYourAnswers");
                }


                //Finally, No Errors so load the next page
                return RedirectToAction("Index", new { id = nextPageId });

            }
            catch (Exception ex)
            {
                //TODO: log error
                return StatusCode(500);
            }

        }

    }
}