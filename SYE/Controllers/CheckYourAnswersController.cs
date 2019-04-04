using System;
using System.Collections.Generic;
using System.Linq;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SYE.Models;
using SYE.Models.SubmissionSchema;
using SYE.Services;

namespace SYE.Controllers
{
    public class CheckYourAnswersController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly ISubmissionService _submissionService;

        public CheckYourAnswersController(ISessionService sessionService, ISubmissionService submissionService)
        {
            _sessionService = sessionService;
            _submissionService = submissionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var formVm = _sessionService.GetFormVmFromSession();
                if (formVm == null)
                {
                    return NotFound();
                }
                var vm = new CheckYourAnswersVm
                {
                    FormVm = formVm
                };

                return View(vm);
            }
            catch (Exception e)
            {
                //log error
                return StatusCode(500);
            }
        }


        [HttpPost]
        public IActionResult Index(CheckYourAnswersVm vm)
        {
            try
            {
                var formVm = _sessionService.GetFormVmFromSession();

                if (formVm == null)
                {
                    return NotFound();
                }

                var submission = GenerateSubmission(formVm);

                var result = _submissionService.CreateAsync(submission);
                var reference = result.Id.ToString();

                if (vm.SendConfirmationEmail)
                {
                    //TODO: Send the confirmation email
                }

                HttpContext.Session.Clear();
                HttpContext.Session.SetString("ReferenceNumber", reference);

                return RedirectToAction("Index", "Confirmation");
            }
            catch (Exception e)
            {
               //log error
               return StatusCode(500);
            }
        }

        private SubmissionVM GenerateSubmission(FormVM formVm)
        {
            var vm = new SubmissionVM
            {
                Version = formVm.Version,
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                ProviderId = HttpContext.Session.GetString("ProviderId"),
                LocationId = HttpContext.Session.GetString("LocationId"),
                LocationName = HttpContext.Session.GetString("LocationName")
            };

            var answers = new List<AnswerVM>();

            foreach (var page in formVm.Pages)
            {
                answers.AddRange(page.Questions.Where(m => !string.IsNullOrEmpty(m.Answer))
                    .Select(question => new AnswerVM
                    {
                        PageId = page.PageId,
                        QuestionId = question.QuestionId,
                        Question = string.IsNullOrEmpty(question.Question) ? page.PageName : question.Question,
                        Answer = question.Answer
                    }));
            }

            vm.Answers = answers;

            return vm;
        }
        
    }
}