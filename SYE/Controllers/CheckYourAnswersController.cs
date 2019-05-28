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
using Microsoft.Extensions.Options;
using SYE.Helpers;
using SYE.Models;
using SYE.Models.SubmissionSchema;
using SYE.Repository;
using SYE.Services;
using SYE.ViewModels;
using SYE.ViewModels.CheckYourAnswers;
using PageVM = GDSHelpers.Models.FormSchema.PageVM;

namespace SYE.Controllers
{
    public class CheckYourAnswersController : BaseController<CheckYourAnswersController>
    {
        private readonly ILogger _logger;
        private readonly ISubmissionService _submissionService;
        private readonly IGovUkNotifyConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly IDocumentService _documentService;
        private readonly IOptions<ApplicationSettings> _config;
        private readonly ISessionService _sessionService;

        public CheckYourAnswersController(IHttpContextAccessor context, IServiceProvider service, IOptions<ApplicationSettings> config,
            ISessionService sessionService) : base(context)
        {
            _submissionService = service.GetService<ISubmissionService>();
            _configuration = service.GetService<IGovUkNotifyConfiguration>();
            _notificationService = service.GetService<INotificationService>();
            _documentService = service.GetService<IDocumentService>();
            _logger = service?.GetRequiredService<ILogger<CheckYourAnswersController>>() as ILogger;
            _config = config;
            _sessionService = sessionService;
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
                    FormVm = formVm,
                    SendConfirmationEmail = true,
                    LocationName = _sessionService.GetUserSession().LocationName,
                    PageHistory =  _sessionService.GetNavOrder()
                    
                };

                ViewBag.ShowBackButton = false;
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Internal server error!");
                return StatusCode(500);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
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
                var result = _submissionService.CreateAsync(submission).Result;
                var reference = submission.SubmissionId ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(reference))  //&& vm?.SendConfirmationEmail == true)
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
                DateCreated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                FormName = formVm.FormName,
                ProviderId = HttpContext.Session.GetString("ProviderId"),
                LocationId = HttpContext.Session.GetString("LocationId"),
                LocationName = HttpContext.Session.GetString("LocationName"),
                SubmissionId = _submissionService.GenerateUniqueUserRefAsync().Result.ToString(),
            };


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
            var emailTemplateId = string.Empty;
            if (string.IsNullOrWhiteSpace(submission?.LocationId) || string.IsNullOrWhiteSpace(submission?.LocationName))
            {
                emailTemplateId = _configuration.WithoutLocationEmailTemplateId;
            }
            else
            {
                emailTemplateId = _configuration.WithLocationEmailTemplateId;
            }
            var greetingTemplate = _configuration.GreetingTemplate;
            var clientReferenceTemplate = _configuration.ClientReferenceTemplate;
            var emailReplyToId = _configuration.ReplyToAddressId;

            var emailAddress = submission?
                .Answers?.FirstOrDefault(x => x.Question.Equals("Your contact details", StringComparison.OrdinalIgnoreCase) && x.QuestionId == _config.Value.UsersEmailField)?
                .Answer ?? string.Empty;

            var feedbackUserName = submission?
                .Answers?.FirstOrDefault(x => x.Question.Equals("Your contact details", StringComparison.OrdinalIgnoreCase) && x.QuestionId == _config.Value.UsersNameField)?
                .Answer ?? string.Empty;

            var greeting = string.Format(greetingTemplate, feedbackUserName);
            var locationName = submission?.LocationName;
            var clientReference = string.Format(clientReferenceTemplate, submission?.LocationId, submission?.Id);

            var personalisation =
                new Dictionary<string, dynamic> {
                    { "greeting", greeting }, { "location", locationName }, {"reference number", submission?.SubmissionId ?? string.Empty }
                };


            if(!string.IsNullOrEmpty(emailAddress)) {
                await _notificationService.NotifyByEmailAsync(
                    emailTemplateId, emailAddress, personalisation, clientReference, emailReplyToId
                ).ConfigureAwait(false);
            }
            
        }
    }
}