﻿using System.Linq;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SYE.Services;

namespace SYE.Controllers
{
    public class FormController : Controller
    {
        private readonly IGdsValidation _gdsValidate;
        private readonly IPageService _pageService;
        private readonly ISessionService _sessionService;

        public FormController(IGdsValidation gdsValidate, IPageService pageService, ISessionService sessionService)
        {
            _gdsValidate = gdsValidate;
            _pageService = pageService;
            _sessionService = sessionService;
        }


        [HttpGet]
        public IActionResult Index(string id = "")
        {
            //Get the form schema from User Session
            var getUserForm = _sessionService.GetFormVmFromSession();



            var pageVm = _pageService.GetPageById(getUserForm, id);
            return View(pageVm);


            //try
            //{
            //    var pageVm = _pageService.GetPageById(id, "Content/form-schema.json", locationName);
            //    if (pageVm == null)
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        return View(pageVm);
            //    }

            //}
            //catch (Exception e)
            //{
            //    //log error
            //    return StatusCode(500);
            //}            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(PageVM vm)
        {
            var locationName = string.Empty;

            if (HttpContext != null)
            {
                locationName = HttpContext.Session.GetString("LocationName");
            }            

            //Get the page we are validating
            var pageVm = _pageService.GetPageById(vm.PageId, "Content/form-schema.json", locationName);

            //Validate the Response against the page json
            _gdsValidate.ValidatePage(pageVm, Request.Form);

            //Get the error count
            var errorCount = pageVm.Questions.Count(m => m.Validation.IsErrored);

            //Build the submission schema
            //var submission = _gdsValidate.BuildSubmission(pageVm);
            //Todo: Store the submission in our DB


            //If we have errors return to the View
            if (errorCount > 0) return View(pageVm);


            //No errors redirect to the Index page with our new PageId
            var nextPageId = pageVm.NextPageId;
            return RedirectToAction("Index", new { id = nextPageId });
        }

    }
}