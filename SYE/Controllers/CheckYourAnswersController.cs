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

namespace SYE.Controllers
{
    public class CheckYourAnswersController : Controller
    {
        private readonly ILogger _logger;
        private readonly ISubmissionService _submissionService;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly IDocumentService _documentService;
        private readonly IOptions<ApplicationSettings> _config;
        private readonly ISessionService _sessionService;

        public CheckYourAnswersController(IServiceProvider service)
        {
            _logger = service?.GetRequiredService<ILogger<CheckYourAnswersController>>() as ILogger;
            _sessionService = service?.GetRequiredService<ISessionService>() ?? null;
            _submissionService = service.GetRequiredService<ISubmissionService>();
            _configuration = service?.GetRequiredService<IConfiguration>();
            _notificationService = service.GetRequiredService<INotificationService>();
            _documentService = service.GetRequiredService<IDocumentService>();
        }

        [HttpGet, Route("form/check-your-answers")]
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

                //ViewBag.ShowBackButton = false;
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading FormVM.");
                return StatusCode(500);
            }

        }


        [HttpPost, Route("form/check-your-answers")]
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
                var reference = _submissionService.GenerateUniqueUserRefAsync().Result.ToString();

                if (string.IsNullOrWhiteSpace(reference))
                {
                    _logger.LogError("Error submitting feedback!  Null or empty submission Id");
                    return StatusCode(500);
                }

                submission.SubmissionId = reference;
                submission = _submissionService.CreateAsync(submission).Result;

                if (vm?.SendConfirmationEmail == true && !string.IsNullOrWhiteSpace(reference))
                {
                    var fieldMappings = _configuration
                        .GetSection("EmailNotification:ConfirmationEmail:FieldMappings")
                        .Get<IEnumerable<EmailFieldMapping>>();

                    var feedbackUserName = submission?
                        .Answers?
                        .FirstOrDefault(x => x.QuestionId.Equals(fieldMappings.FirstOrDefault(y => y.Name == "name")?.FormField, StringComparison.OrdinalIgnoreCase))?
                        .Answer ?? string.Empty;

                    var emailAddress = submission?
                        .Answers?
                        .FirstOrDefault(x => x.QuestionId.Equals(fieldMappings.FirstOrDefault(y => y.Name == "email")?.FormField, StringComparison.OrdinalIgnoreCase))?
                        .Answer ?? string.Empty;

                    if (!string.IsNullOrWhiteSpace(emailAddress))
                    {

                        var locationId = submission?.LocationId;
                        var locationName = submission?.LocationName;
                        var submissionId = submission?.Id;

                        Task.Run(async () =>
                        {
                            await SendEmailNotificationAsync(feedbackUserName, emailAddress, locationId, locationName, submissionId, reference)
                                    .ContinueWith(notificationTask =>
                                    {
                                        if (notificationTask.IsFaulted)
                                        {
                                            _logger.LogError(notificationTask.Exception, $"Error sending confirmation email with submission id: [{reference}].");
                                        }
                                        else
                                        {
                                            _logger.LogInformation($"Confirmation email for submission id: [{reference}] sent successfully.");
                                        }
                                    })
                                    .ConfigureAwait(false);
                        });
                    }
                }

                HttpContext.Session.Clear();
                HttpContext.Session.SetString("ReferenceNumber", reference);

                return RedirectToAction("Index", "Confirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting feedback!");
                return StatusCode(500);
            }
        }

        private SubmissionVM GenerateSubmission(FormVM formVm)
        {
            var submissionVm = new SubmissionVM
            {
                Version = formVm.Version,
                Id = Guid.NewGuid().ToString(),
                DateCreated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                FormName = formVm.FormName,
                ProviderId = HttpContext.Session.GetString("ProviderId"),
                LocationId = HttpContext.Session.GetString("LocationId"),
                LocationName = HttpContext.Session.GetString("LocationName")                
            };

            var answers = new List<AnswerVM>();

            var pageHistory = _sessionService.GetNavOrder();
            foreach (var page in formVm.Pages.Where(m => pageHistory.Contains(m.PageId)).OrderBy(m => pageHistory.IndexOf(m.PageId)))
            {
                answers.AddRange(page.Questions.Where(m => !string.IsNullOrEmpty(m.Answer))
                    .Select(question => new AnswerVM
                    {
                        PageId = page.PageId,
                        QuestionId = question.QuestionId,
                        Question = string.IsNullOrEmpty(question.Question) ? page.PageName.StripHtml() : question.Question.StripHtml(),
                        Answer = question.Answer.StripHtml(),
                        DocumentOrder = question.DocumentOrder
                    }));
            }

            submissionVm.Answers = answers;

            submissionVm.Base64Attachment = _documentService.CreateSubmissionDocument(submissionVm);
            submissionVm.Status = "Saved";

            return submissionVm;
        }

        private async Task SendEmailNotificationAsync(string fullName, string emailAddress, string locationId, string locationName, string submissionId, string submissionReference)
        {
            var emailTemplateId = string.Empty;
            if (string.IsNullOrWhiteSpace(locationId.Replace("0", "")))
            {
                emailTemplateId = _configuration.GetSection("EmailNotification:ConfirmationEmail").GetValue<string>("WithoutLocationEmailTemplateId");
            }
            else
            {
                emailTemplateId = _configuration.GetSection("EmailNotification:ConfirmationEmail").GetValue<string>("WithLocationEmailTemplateId");
            }

            var greetingTemplate = _configuration.GetSection("EmailNotification:ConfirmationEmail").GetValue<string>("GreetingTemplate");
            var clientReferenceTemplate = _configuration.GetSection("EmailNotification:ConfirmationEmail").GetValue<string>("ClientReferenceTemplate");
            var emailReplyToId = _configuration.GetSection("EmailNotification:ConfirmationEmail").GetValue<string>("ReplyToAddressId");

            var greeting = string.Format(greetingTemplate, fullName);
            var clientReference = string.Format(clientReferenceTemplate, locationId, submissionId);

            var personalisation =
                new Dictionary<string, dynamic> {
                    { "greeting", greeting }, { "location", locationName }, {"reference number", submissionReference ?? string.Empty }
                };


            if(!string.IsNullOrEmpty(emailAddress)) {
                await _notificationService.NotifyByEmailAsync(
                    emailTemplateId, emailAddress, personalisation, clientReference, emailReplyToId
                ).ConfigureAwait(false);
            }
            
        }
    }
}