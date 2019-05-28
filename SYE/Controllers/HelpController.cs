using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SYE.Services;

namespace SYE.Controllers
{
    public class HelpController : Controller
    {
        private readonly ILogger _logger;
        private readonly IFormService _formService;
        private readonly IGdsValidation _gdsValidate;
        private readonly IConfiguration _configuration;
        private readonly INotificationService _notificationService;

        public HelpController(IServiceProvider service)
        {
            this._logger = service?.GetService<ILogger<HelpController>>() as ILogger;
            this._formService = service?.GetService<IFormService>();
            this._gdsValidate = service?.GetService<IGdsValidation>();
            this._configuration = service?.GetService<IConfiguration>();
            this._notificationService = service?.GetService<INotificationService>();
        }

        [HttpGet("report-a-problem")]
        public ActionResult Feedback([FromHeader(Name = "referer")] string urlReferer)
        {
            try
            {
                var pageViewModel = GetPage();
                if (pageViewModel != null)
                {
                    ViewBag.UrlReferer = urlReferer;
                    return View(nameof(Feedback), pageViewModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading service feedback form.");
            }
            return StatusCode(500);
        }

        [HttpPost("report-a-problem"), ActionName("Feedback")]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitFeedback([FromForm(Name = "url-referer")] string urlReferer)
        {
            PageVM pageViewModel = null;
            try
            {
                pageViewModel = GetPage();

                if (pageViewModel == null)
                {
                    return StatusCode(500);
                }

                _gdsValidate.ValidatePage(pageViewModel, Request.Form);

                if (pageViewModel.Questions.Any(m => m.Validation?.IsErrored == true))
                {
                    ViewBag.ShowBackButton = false;
                    ViewBag.UrlReferer = urlReferer;
                    return View(nameof(Feedback), pageViewModel);
                }

                Task.Run(async () =>
                {
                    await SendEmailNotificationAsync(pageViewModel, urlReferer)
                            .ContinueWith(notificationTask =>
                                _logger.LogError(notificationTask.Exception, "Error sending service feedback email."),
                                TaskContinuationOptions.OnlyOnFaulted
                            )
                            .ConfigureAwait(false);
                });

                return RedirectToAction(nameof(FeedbackThankYou));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting service feedback.");
            }

            return StatusCode(500);
        }

        [Route("feedback-thank-you")]
        public ActionResult FeedbackThankYou()
        {
            ViewBag.ShowBackButton = false;
            return View();
        }

        private PageVM GetPage()
        {
            string formName = _configuration.GetSection("FormsConfiguration:ServiceFeedbackForm").GetValue<String>("Name");
            string version = _configuration.GetSection("FormsConfiguration:ServiceFeedbackForm").GetValue<String>("Version");

            FormVM form = string.IsNullOrEmpty(version) ?
                _formService.GetLatestFormByName(formName).Result :
                _formService.FindByNameAndVersion(formName, version).Result;

            return form.Pages.FirstOrDefault() ?? null;
        }

        private Task SendEmailNotificationAsync(PageVM submission, String urlReferer)
        {
            if (submission == null)
            {
                throw new ArgumentNullException(nameof(submission));
            }
            return SendEmailNotificationInternalAsync(submission, urlReferer);
        }

        private async Task SendEmailNotificationInternalAsync(PageVM submission, String urlReferer)
        {
            IConfigurationSection config = _configuration.GetSection("GovUkNotifyConfiguration:FeedbackEmail");

            string emailTemplateId = config.GetValue<String>("EmailTemplateId");
            string emailAddress = config.GetValue<String>("ServiceSupportEmailAddress");
            string phase = config.GetValue<String>("Phase");

            string feedbackMessage = submission?
                .Questions?.FirstOrDefault(x => x.QuestionId.Equals("message", StringComparison.OrdinalIgnoreCase))?
                .Answer ?? String.Empty;
            string feedbackUserName = submission?
                .Questions?.FirstOrDefault(x => x.QuestionId.Equals("full-name", StringComparison.OrdinalIgnoreCase))?
                .Answer ?? String.Empty;
            string feedbackUserEmailAddress = submission?
                .Questions?.FirstOrDefault(x => x.QuestionId.Equals("email-address", StringComparison.OrdinalIgnoreCase))?
                .Answer ?? String.Empty;

            Dictionary<string, dynamic> personalisation =
                new Dictionary<string, dynamic> {
                    { "service-phase", phase },
                    { "banner-clicked-on-page", urlReferer },
                    { "feedback-message", feedbackMessage },
                    { "feedback-full-name", feedbackUserName },
                    { "feedback-email-address", feedbackUserEmailAddress }
                };

            await _notificationService.NotifyByEmailAsync(
                    emailTemplateId, emailAddress, personalisation, null, null
                ).ConfigureAwait(false);
        }
    }
}