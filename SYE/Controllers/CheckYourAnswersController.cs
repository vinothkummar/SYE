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
            var vm = new CheckYourAnswersVm
            {
                FormVm = _sessionService.GetFormVmFromSession()
            };

            return View(vm);
        }


        [HttpPost]
        public IActionResult Index(CheckYourAnswersVm vm)
        {
            var formVm = _sessionService.GetFormVmFromSession();

            var submission = GenerateSubmission(formVm);

            var result = _submissionService.CreateAsync(submission);
            var reference = result.Id;

            if (vm.SendConfirmationEmail)
            {
                //TODO: Send the confirmation email
            }
            
            return RedirectToAction("Index", "Confirmation", new { id = reference });
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