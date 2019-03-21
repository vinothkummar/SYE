using System;
using System.Linq;
using System.Net;
using System.Text;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            var locationName = string.Empty;
            if (HttpContext != null)
            {
                HttpContext.Session.SetString("LocationId", "1-100000001");
                HttpContext.Session.SetString("LocationName", "The Thatched House Dental Practise");
                locationName = HttpContext.Session.GetString("LocationName");
            }

            try
            {
                PageVM pageVm = GetpageVM(locationName, id);

                if (pageVm != null)
                {
                    return View(pageVm);
                }

                return NotFound();
            }
            catch (Exception)
            {
                //log error
                return StatusCode(500);
            }
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
            var pageVm = GetpageVM(locationName, vm.PageId);

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

        private FormVM GetFormVM(String locationName)
        {
            FormVM result = null;
            String form_schema_key = "_formVM_";
            byte[] encodedFormVM = null;
            if (HttpContext.Session.TryGetValue(form_schema_key, out encodedFormVM) == false || encodedFormVM == null)
            {
                result = _pageService.GetLatestForm().Result;
                encodedFormVM = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result).Replace("!!location_name!!", locationName));
                HttpContext.Session.Set(form_schema_key, encodedFormVM);
            }
            result = JsonConvert.DeserializeObject<FormVM>(Encoding.UTF8.GetString(encodedFormVM));
            return result;
        }

        private PageVM GetpageVM(String locationName, string pageId)
        {
            PageVM result = null;
            FormVM formVM = this.GetFormVM(locationName);

            if (formVM != null)
            {
                if (string.IsNullOrWhiteSpace(pageId))
                {
                    result = formVM.Pages.FirstOrDefault();
                }
                if (formVM.Pages.Any(x => x.PageId == pageId) == true)
                {
                    result = formVM.Pages.FirstOrDefault(x => x.PageId == pageId);
                }
            }

            return result;
        }
    }
}