﻿using System;
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
    public class CheckYourAnswersController : Controller
    {
        private readonly ILogger _logger;
        private readonly ISessionService _sessionService;
        private readonly ISubmissionService _submissionService;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;
        private readonly IDocumentService _documentService;

        public CheckYourAnswersController(IServiceProvider service)
        {
            _logger = service?.GetRequiredService<ILogger<CheckYourAnswersController>>() as ILogger;
            _sessionService = service?.GetRequiredService<ISessionService>() ?? null;
            _submissionService = service.GetRequiredService<ISubmissionService>();
            _configuration = service?.GetRequiredService<IConfiguration>();
            _notificationService = service.GetRequiredService<INotificationService>();
            _documentService = service.GetRequiredService<IDocumentService>();
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
                    SendConfirmationEmail = true
                };

                ViewBag.ShowBackButton = false;
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading FormVM.");
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
                submission = _submissionService.CreateAsync(submission).Result;
                var reference = submission?.SubmissionId ?? string.Empty;

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
            var test = 0;
            foreach (var page in formVm.Pages)
            {
                answers.AddRange(page.Questions.Where(m => !string.IsNullOrEmpty(m.Answer))
                    .Select(question => new AnswerVM
                    {
                        PageId = page.PageId,
                        QuestionId = question.QuestionId,
                        Question = string.IsNullOrEmpty(question.Question) ? page.PageName.StripHtml() : question.Question.StripHtml(),
                        Answer = question.Answer.StripHtml(),//.RemoveLineBreaks()
                        DocumentOrder = test++
                    }));
            }

            vm.Answers = answers;

            vm.Base64Attachment = _documentService.CreateSubmissionDocument(vm);
            vm.Status = "Saved";

            return vm;
        }

        private async Task SendEmailNotificationAsync(string fullName, string emailAddress, string locationId, string locationName, string submissionId, string submissionReference)
        {
            var emailTemplateId = string.Empty;
            if (string.IsNullOrWhiteSpace(locationId) || string.IsNullOrWhiteSpace(locationName))
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
                    { "greeting", greeting }, { "location", locationName }, {"reference number", submissionReference ?? String.Empty }
                };

            await _notificationService.NotifyByEmailAsync(
                    emailTemplateId, emailAddress, personalisation, clientReference, emailReplyToId
                ).ConfigureAwait(false);
        }
    }
}