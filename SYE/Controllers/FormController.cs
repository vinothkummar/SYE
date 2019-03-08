using System.IO;
using System.Linq;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Session;
using Newtonsoft.Json;

namespace SYE.Controllers
{
    public class FormController : Controller
    {
        private readonly IGdsValidation _gdsValidate;

        public FormController(IGdsValidation gdsValidate)
        {
            _gdsValidate = gdsValidate;
        }


        [HttpGet]
        public IActionResult Index(string id = "")
        {
            HttpContext.Session.SetString("LocationId", "1-100000001");
            HttpContext.Session.SetString("LocationName", "The Thatched House Dental Practise");

            var locationName = HttpContext.Session.GetString("LocationName");

            var pageVm = GetPageById(id, locationName);
            return View(pageVm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(PageVM vm)
        {
            var locationName = HttpContext.Session.GetString("LocationName");

            //Get the page we are validating
            var pageVm = GetPageById(vm.PageId, locationName);

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


        private static PageVM GetPageById(string pageId, string locationName = "")
        {
            FormVM formVm;
            using (var r = new StreamReader("Content/form-schema.json"))
            {
                var file = r.ReadToEnd();

                file = file.Replace("!!location_name!!", locationName);

                formVm = JsonConvert.DeserializeObject<FormVM>(file);
            }

            var pageVm = string.IsNullOrEmpty(pageId)
                ? formVm.Pages.FirstOrDefault()
                : formVm.Pages.FirstOrDefault(m => m.PageId == pageId);

            return pageVm;
        }

    }
}