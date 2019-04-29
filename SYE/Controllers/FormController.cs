using System;
using System.Linq;
using GDSHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SYE.Models;
using SYE.Services;

namespace SYE.Controllers
{
    public class FormController : Controller
    {
        private readonly IGdsValidation _gdsValidate;
        private readonly ISessionService _sessionService;

        public FormController(IGdsValidation gdsValidate, ISessionService sessionService)
        {
            _gdsValidate = gdsValidate;
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index(string id = "")
        {
            try
            {
                if (HttpContext?.Session != null)
                {
                    ViewBag.LocationName = HttpContext.Session.GetString("LocationName");
                    ViewBag.NotFoundDetails = HttpContext.Session.GetString("NotFoundDetails");
                }

                if (id == "" && ViewBag.LocationName == "Not Found")
                {
                    id = "NotFound";
                }
                var pageVm = _sessionService.GetPageById(id);

                if (pageVm != null)
                {
                    if (!String.IsNullOrEmpty(pageVm.PreviousPageId))
                    {
                        if (pageVm.PreviousPageId.Equals("start", StringComparison.InvariantCultureIgnoreCase))
                        {
                            ViewBag.PreviousPage = "/search";
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
                //TODO: log error
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
                var pageVm = _sessionService.GetPageById(vm.PageId);

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
                if (nextPageId == "CheckYourAnswers") return RedirectToAction("Index", "CheckYourAnswers");

                //Finally, No Errors so load the next page
                return RedirectToAction("Index", new { id = nextPageId });

            }
            catch (Exception ex)
            {
                //TODO: log error
                return StatusCode(500);
            }

        }

    }
}
