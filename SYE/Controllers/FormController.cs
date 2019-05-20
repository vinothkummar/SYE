using System;
using System.Linq;
using GDSHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SYE.Models;
using SYE.Services;

namespace SYE.Controllers
{
    public class FormController : Controller
    {
        private readonly IGdsValidation _gdsValidate;
        private readonly ISessionService _sessionService;
        private readonly ILogger _logger;

        public FormController(IGdsValidation gdsValidate, ISessionService sessionService, ILogger<FormController> logger)
        {
            _gdsValidate = gdsValidate;
            _sessionService = sessionService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(string id = "")
        {
            try
            {
                if (HttpContext?.Session != null)
                {
                    ViewBag.LocationName = HttpContext.Session.GetString("LocationName");
                }

                var notFoundFirstPageFlag = (bool)(id == "" && ViewBag.LocationName == "the service");

                var pageVm = _sessionService.GetPageById(id, notFoundFirstPageFlag);

                if (pageVm != null)
                {
                    if (!String.IsNullOrEmpty(pageVm.PreviousPageId))
                    {
                        if (pageVm.PreviousPageId.Equals("start", StringComparison.InvariantCultureIgnoreCase))
                        {
                            if ((!notFoundFirstPageFlag) && ViewBag.LocationName == "the service")
                            {
                                //this is the SECOND page of the journey when location is NOT found
                                //However its also the FIRST page of the journey when the location IS found
                                //back button should go to Not Found details page
                                ViewBag.PreviousPage = String.Concat("/Form/Index/", "");
                            }
                            else
                            {
                                //this is the FIRST page of the journey
                                //so back button should go to the search page
                                ViewBag.PreviousPage = "/search";
                            }
                        }
                        else
                        {
                            ViewBag.PreviousPage = String.Concat("/Form/Index/", pageVm.PreviousPageId);
                        }
                    }
                    return View(pageVm);
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading PageVM.");
                return StatusCode(500);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CurrentPageVM vm)
        {
            try
            {
                if (HttpContext?.Session != null)
                {
                    ViewBag.LocationName = HttpContext.Session.GetString("LocationName");
                }

                //Get the current PageVm from Session
                var pageVm = _sessionService.GetPageById(vm.PageId, false);

                //If Null throw NotFound error
                if (pageVm == null) return NotFound();

                if (!String.IsNullOrEmpty(pageVm.PreviousPageId))
                {
                    ViewBag.PreviousPage = String.Concat("/Form/Index/", pageVm.PreviousPageId);
                }

                if (Request?.Form != null)
                {
                    //Validate the Response against the page json and update PageVm to contain the answers
                    _gdsValidate.ValidatePage(pageVm, Request.Form);
                }

                //Get the error count
                var errorCount = pageVm.Questions?.Count(m => m.Validation != null && m.Validation.IsErrored);

                //If we have errors return to the View
                if (errorCount > 0) return View(pageVm);

                //Now we need to update the FormVM in session.
                _sessionService.UpdatePageVmInFormVm(pageVm);

                //No errors redirect to the Index page with our new PageId
                var nextPageId = pageVm.NextPageId;

                //Check the nextPageId for preset controller names
                switch (nextPageId)
                {
                    case "CheckYourAnswers":
                        return RedirectToAction("Index", "CheckYourAnswers");

                    case "Home":
                        return RedirectToAction("Index", "Home");
                }

                //Finally, No Errors so load the next page
                return RedirectToAction("Index", new { id = nextPageId });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating PageVM.");
                return StatusCode(500);
            }
        }

    }
}
