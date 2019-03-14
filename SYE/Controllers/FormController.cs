using System.Linq;
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

        public FormController(IGdsValidation gdsValidate, IPageService pageService)
        {
            _gdsValidate = gdsValidate;
            _pageService = pageService;
        }


        [HttpGet]
        public IActionResult Index(string id = "")
        {
            HttpContext.Session.SetString("LocationId", "1-100000001");
            HttpContext.Session.SetString("LocationName", "The Thatched House Dental Practise");

            var locationName = HttpContext.Session.GetString("LocationName");

            var pageVm = _pageService.GetPageById(id, "Content/form-schema.json", locationName);
            return View(pageVm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(PageVM vm)
        {
            var locationName = HttpContext.Session.GetString("LocationName");

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