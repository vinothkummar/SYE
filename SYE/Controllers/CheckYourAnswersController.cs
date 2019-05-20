using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SYE.Helpers;
using SYE.Models;
using SYE.Models.SubmissionSchema;
using SYE.Repository;
using SYE.Services;

namespace SYE.Controllers
{
    public class CheckYourAnswersController : BaseController<CheckYourAnswersController>
    {
        private readonly ISubmissionService _submissionService;
        private readonly IDocumentService _documentService;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;

        public CheckYourAnswersController(IHttpContextAccessor context, IServiceProvider service) : base(context)
        {
            this._submissionService = service.GetService<ISubmissionService>();
            this._documentService = service.GetService<IDocumentService>();
            this._configuration = service?.GetService<IConfiguration>();
            this._notificationService = service.GetService<INotificationService>();
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
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading FormVM.");
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
                submission = _submissionService.CreateAsync(submission).Result;
                var reference = submission.SubmissionId ?? string.Empty;

                if (vm?.SendConfirmationEmail == true && !string.IsNullOrWhiteSpace(reference))
                {
                    Task.Run(async () =>
                    {
                        await SendEmailNotificationAsync(submission)
                                .ContinueWith(notificationTask =>
                                    {
                                        if (notificationTask.IsFaulted)
                                        {
                                            Logger.LogError(notificationTask.Exception, $"Error sending confirmation email with submission id: [{reference}].");
                                        }
                                        else
                                        {
                                            Logger.LogInformation($"Confirmation email for submission id: [{reference}] sent successfully.");
                                        }
                                    })
                                .ConfigureAwait(false);
                    });
                }

                HttpContext.Session.Clear();
                HttpContext.Session.SetString("ReferenceNumber", reference);

                return RedirectToAction("Index", "Confirmation");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error submitting feedback!");
                return StatusCode(500);
            }
        }

        private SubmissionVM GenerateSubmission(FormVM formVm)
        {
            var vm = new SubmissionVM
            {
                Version = formVm.Version,
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                FormName = formVm.FormName,
                ProviderId = HttpContext.Session.GetString("ProviderId"),
                LocationId = HttpContext.Session.GetString("LocationId"),
                LocationName = HttpContext.Session.GetString("LocationName"),
            };

            vm.SubmissionId = _submissionService.GenerateUniqueUserRefAsync().Result.ToString();

            var answers = new List<AnswerVM>();

            foreach (var page in formVm.Pages)
            {
                answers.AddRange(page.Questions.Where(m => !string.IsNullOrEmpty(m.Answer))
                    .Select(question => new AnswerVM
                    {
                        PageId = page.PageId,
                        QuestionId = question.QuestionId,
                        Question = string.IsNullOrEmpty(question.Question) ? page.PageName.StripHtml() : question.Question.StripHtml(),
                        Answer = question.Answer.StripHtml().RemoveLineBreaks()
                    }));
            }

            vm.Answers = answers;
            vm.Base64Attachment = _documentService.CreateSubmissionDocument(vm);
            vm.Status = "Saved";

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
            var fieldMappings = _configuration.GetSection("EmailNotification:ConfirmationEmail:FieldMappings").Get<IEnumerable<EmailFieldMapping>>();
            string emailTemplateId = String.Empty;
            if (String.IsNullOrWhiteSpace(submission?.LocationId) || String.IsNullOrWhiteSpace(submission?.LocationName))
            {
                emailTemplateId = _configuration.GetSection("EmailNotification:ConfirmationEmail").GetValue<string>("WithoutLocationEmailTemplateId");
            }
            else
            {
                emailTemplateId = _configuration.GetSection("EmailNotification:ConfirmationEmail").GetValue<string>("WithLocationEmailTemplateId");
            }
            string greetingTemplate = _configuration.GetSection("EmailNotification:ConfirmationEmail").GetValue<string>("GreetingTemplate");
            string clientReferenceTemplate = _configuration.GetSection("EmailNotification:ConfirmationEmail").GetValue<string>("ClientReferenceTemplate");
            string emailReplyToId = _configuration.GetSection("EmailNotification:ConfirmationEmail").GetValue<string>("ReplyToAddressId");

            string emailAddress = submission?
                .Answers?.FirstOrDefault(x => x.QuestionId.Equals(fieldMappings.FirstOrDefault(y => y.Name == "email").FormField, StringComparison.OrdinalIgnoreCase))?
                .Answer ?? String.Empty;

            string feedbackUserName = submission?
                .Answers?.FirstOrDefault(x => x.QuestionId.Equals(fieldMappings.FirstOrDefault(y => y.Name == "name").FormField, StringComparison.OrdinalIgnoreCase))?
                .Answer ?? String.Empty;

            var greeting = string.Format(greetingTemplate, feedbackUserName);
            var locationName = submission?.LocationName;
            var clientReference = string.Format(clientReferenceTemplate, submission?.LocationId, submission?.Id);

            var personalisation =
                new Dictionary<string, dynamic> {
                    { "greeting", greeting }, { "location", locationName }, {"reference number", submission?.SubmissionId ?? String.Empty }
                };

            await _notificationService.NotifyByEmailAsync(
                    emailTemplateId, emailAddress, personalisation, clientReference, emailReplyToId
                ).ConfigureAwait(false);
        }
    }
}