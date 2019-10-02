﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SYE.Controllers
{
    public class ErrorController : Controller
    {
        public ILogger<ErrorController> _logger { get; }

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [Route("Error/{statusCode}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult HttpStatusCodeHandler(int? statusCode = null)
        {
            var statusCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            var responsefeature = HttpContext.Features.Get<IHttpResponseFeature>();
            var message = responsefeature.ReasonPhrase;

            ViewBag.Path = statusCodeResult?.OriginalPath;
            ViewBag.QueryString = statusCodeResult?.OriginalQueryString;

            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            switch (statusCode)
            {
                case 500:                
                    _logger.LogError("500 Type Error Occured. " +
                                     $"The message {exceptionDetails.Error.Data?["GFCError"]} " +
                                     $"The path {exceptionDetails.Path} " +
                                     $"threw an exception: {exceptionDetails.Error}");
                    return View("GenericException");
                case 550:
                    _logger.LogCritical($"{statusCode} Search Error Occured. " +
                                     $"Message = {message}, " +
                                     $"Path = {statusCodeResult?.OriginalPath}, " +
                                     $"QueryString = {statusCodeResult?.OriginalQueryString}");
                    return View("GenericException");
                case 551:
                case 552:
                case 555:
                case 556:
                    _logger.LogCritical($"{statusCode} Form json Not Found Error Occured. " +
                                     $"Message = {message}, " +
                                     $"Path = {statusCodeResult?.OriginalPath}, " +
                                     $"QueryString = {statusCodeResult?.OriginalQueryString}");
                    return View("GenericException");
                case 560:
                case 561:
                    _logger.LogWarning($"{statusCode} Form Page Load Session Error Occured. " +
                                     $"Message = {message}, " +
                                     $"Path = {statusCodeResult?.OriginalPath}, " +
                                     $"QueryString = {statusCodeResult?.OriginalQueryString}");
                    return View("SessionNotFound");
                case 562:
                    _logger.LogError($"{statusCode} Form Page Load Session Error Occured. " +
                                       $"Message = {message}, " +
                                       $"Path = {statusCodeResult?.OriginalPath}, " +
                                       $"QueryString = {statusCodeResult?.OriginalQueryString}");
                    return View("SessionNotFound");
                case 563:
                case 564:
                case 565:
                    _logger.LogError($"{statusCode} Form Continue Session Error Occured. " +
                                     $"Message = {message}, " +
                                     $"Path = {statusCodeResult?.OriginalPath}, " +
                                     $"QueryString = {statusCodeResult?.OriginalQueryString}");
                    return View("SessionNotFound");
                case 570:
                case 571:                
                    _logger.LogWarning($"{statusCode} Check Your Answers Page Load Session Error Occured. " +
                                     $"Message = {message}, " +
                                     $"Path = {statusCodeResult?.OriginalPath}, " +
                                     $"QueryString = {statusCodeResult?.OriginalQueryString}");
                    return View("SessionNotFound");
                case 572:
                case 573:
                case 574:
                    _logger.LogError($"{statusCode} Check Your Answers Page Submission Error Occured. " +
                                     $"Message = {message}, " +
                                     $"Path = {statusCodeResult?.OriginalPath}, " +
                                     $"QueryString = {statusCodeResult?.OriginalQueryString}");
                    return View("GenericException");
                default:
                    _logger.LogError("Other Type of Error Occured. " +
                                     $"The path {exceptionDetails.Path} " +
                                     $"threw an exception: {exceptionDetails.Error}");
                    return View("GenericError");
            }

        }

    }

}
