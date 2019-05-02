using System;
using System.Collections.Generic;
using System.IO;
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
        private string _dir = Directory.GetCurrentDirectory() + "\\Documents\\";//this will be temporary so dont put into app settings
        private readonly ISubmissionService _submissionService;
        private readonly IGovUkNotifyConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly IDocumentService _documentService;

        public CheckYourAnswersController(IHttpContextAccessor context, IServiceProvider service) : base(context)
        {
            _submissionService = service.GetService<ISubmissionService>();
            _configuration = service.GetService<IGovUkNotifyConfiguration>();
            _notificationService = service.GetService<INotificationService>();
            _documentService = service.GetService<IDocumentService>();
        }

        [HttpGet]
        public IActionResult Index()
        {
            try
            {
                var formVm = SessionService.GetFormVmFromSession();
                if (formVm == null)
                {
                    return NotFound();
                }
                var vm = new CheckYourAnswersVm
                {
                    FormVm = formVm
                };
                ViewBag.ShowBackButton = false;
                return View(vm);
            }
            catch (Exception e)
            {
                //log error
                return StatusCode(500);
            }
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
                var filePath = _documentService.CreateSubmissionDocument(submission, _dir);
                var reference = submission.UserRef ?? String.Empty;

                if (!String.IsNullOrWhiteSpace(reference) && vm?.SendConfirmationEmail == true)
                {
                    using (Logger.BeginScope(new Dictionary<string, object> { { "Submission Reference", reference } }))
                    {
                        try
                        {
                            Task.Run(async () => await SendEmailNotificationAsync(submission).ConfigureAwait(false));
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
                LocationName = HttpContext.Session.GetString("LocationName"),
            };

            vm.UserRef = _submissionService.GenerateUniqueUserRefAsync().Result.ToString();

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

        private Task SendEmailNotificationAsync(SubmissionVM submission)
        {
            if (submission == null)
            {
                throw new ArgumentNullException(nameof(submission));
            }
            return SendEmailNotificationInternalAsync(submission);
        }

        private async Task SendEmailNotificationInternalAsync(SubmissionVM submission)
        {
            string emailTemplateId = String.Empty;
            if (String.IsNullOrWhiteSpace(submission?.LocationId) || String.IsNullOrWhiteSpace(submission?.LocationName))
            {
                emailTemplateId = _configuration.WithoutLocationEmailTemplateId;
            }
            else
            {
                emailTemplateId = _configuration.WithLocationEmailTemplateId;
            }
            string greetingTemplate = _configuration.GreetingTemplate;
            string clientReferenceTemplate = _configuration.ClientReferenceTemplate;
            string emailReplyToId = _configuration.ReplyToAddressId;

            string emailAddress = submission?
                .Answers?.FirstOrDefault(x => x.Question.Equals("Your contact details", StringComparison.OrdinalIgnoreCase) && x.QuestionId == "Contact_003_02")?
                .Answer ?? String.Empty;

            string feedbackUserName = submission?
                .Answers?.FirstOrDefault(x => x.Question.Equals("Your contact details", StringComparison.OrdinalIgnoreCase) && x.QuestionId == "Contact_003_01")?
                .Answer ?? String.Empty;

            string greeting = String.Format(greetingTemplate, feedbackUserName);
            string locationName = submission?.LocationName;
            string clientReference = String.Format(clientReferenceTemplate, submission?.LocationId, submission?.Id);

            Dictionary<string, dynamic> personalisation =
                new Dictionary<string, dynamic> {
                    { "greeting", greeting }, { "location", locationName }, {"reference number", submission?.UserRef ?? String.Empty }
                };

            await _notificationService.NotifyByEmailAsync(
                    emailTemplateId, emailAddress, personalisation, clientReference, emailReplyToId
                ).ConfigureAwait(false);
        }
    }
}