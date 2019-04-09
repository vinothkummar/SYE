using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SYE.Models;
using SYE.Models.SubmissionSchema;
using SYE.Repository;
using SYE.Services;

namespace SYE.Controllers
{
    public class CheckYourAnswersController : BaseController<CheckYourAnswersController>
    {
        private readonly ISubmissionService _submissionService;
        private readonly IGovUkNotifyConfiguration _configuration;
        private readonly INotificationService _notificationService;

        public CheckYourAnswersController(IHttpContextAccessor context, IServiceProvider service) : base(context)
        {
            _submissionService = service.GetService<ISubmissionService>();
            _configuration = service.GetService<IGovUkNotifyConfiguration>();
            _notificationService = service.GetService<INotificationService>();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new CheckYourAnswersVm
            {
                FormVm = SessionService.GetFormVmFromSession()
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CheckYourAnswersVm vm)
        {
            try
            {
                var formVm = SessionService.GetFormVmFromSession();
                if (formVm == null)
                {
                    return NotFound();
                }

                var submission = GenerateSubmission(formVm);
                var result = _submissionService.CreateAsync(submission).Result;
                var reference = result?.Id ?? String.Empty;

                if (!String.IsNullOrWhiteSpace(reference) && vm?.SendConfirmationEmail == true)
                {
                    using (Logger.BeginScope(new Dictionary<string, object> { { "Submission Id", reference } }))
                    {
                        try
                        {
                            Task.Run(() => SendEmailNotification(submission));
                            Logger.LogInformation($"Confirmation email for submission id: [{reference}] sent successfully.");
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(ex, $"Error sending confirmation email with submission id: [{reference}].");
                        }
                    }
                }

                HttpContext.Session.Clear();
                HttpContext.Session.SetString("ReferenceNumber", reference);

                return RedirectToAction("Index", "Confirmation");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Internal server error!");
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

        private async Task SendEmailNotification(SubmissionVM submission)
        {
            string emailAddress = submission?
                .Answers?.FirstOrDefault(x => x.Question.Equals("Email", StringComparison.OrdinalIgnoreCase))?
                .Answer ?? String.Empty;

            if (!String.IsNullOrWhiteSpace(emailAddress))
            {
                string templateId = String.Empty;
                string greetingTemplate = _configuration.GreetingTemplate;
                string feedbackUserName = submission?.Answers?.FirstOrDefault(x => x.Question.Equals("Full name", StringComparison.OrdinalIgnoreCase))?.Answer ?? String.Empty;
                string greeting = String.Format(greetingTemplate, feedbackUserName);
                string locationName = submission?.LocationName;
                Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> { { "greeting", greeting }, { "location", locationName } };
                //TODO: once reference value and format is confirmed, this needs to be updated.
                string clientReferenceTemplate = _configuration.ClientReferenceTemplate;
                string clientReference = String.Format(clientReferenceTemplate, submission?.LocationId, submission?.Id);
                string emailReplyToId = _configuration.ReplyToAddressId;
                if (String.IsNullOrWhiteSpace(submission?.LocationId) || String.IsNullOrWhiteSpace(submission?.LocationName))
                {
                    templateId = _configuration.WithoutLocationEmailTemplateId;
                }
                else
                {
                    templateId = _configuration.WithLocationEmailTemplateId;
                }
                await _notificationService.NotifyByEmailAsync(templateId, emailAddress, personalisation, clientReference, emailReplyToId).ConfigureAwait(false);
            }
        }
    }
}