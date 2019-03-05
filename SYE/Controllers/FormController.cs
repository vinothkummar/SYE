using System.IO;
using System.Linq;
using GDSHelpers;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Mvc;
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
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult Index(string pageId = "")
        {
            var pageVm = GetPageById(pageId);
            return View(pageVm);
        }


        [HttpPost]
        [ResponseCache(NoStore = true, Duration = 0)]
        public IActionResult Index(PageVM vm)
        {
            //Get the page we are validating
            var pageVm = GetPageById(vm.PageId);

            //Validate the Response against the page json
            //_gdsValidate.ValidatePage(pageVm, Request.Form);

            ////Get the error count
            //var errorCount = pageVm.Questions.Count(m => m.Validation.IsErrored);

            ////Build the submission schema
            //var submission = _gdsValidate.BuildSubmission(pageVm);
            ////Todo: Store the submission in our DB

            //if (errorCount > 0)
            //    return View(pageVm);


            var nextPageId = pageVm.NextPageId;
            var nextPageVm = GetPageById(nextPageId);

            ViewBag.PageId = nextPageVm.PageId;


            return View(nextPageVm);
        }


        private static PageVM GetPageById(string pageId)
        {
            FormVM formVm;
            using (var r = new StreamReader("Content/form-schema.json"))
            {
                var file = r.ReadToEnd();
                formVm = JsonConvert.DeserializeObject<FormVM>(file);
            }

            var pageVm = string.IsNullOrEmpty(pageId)
                ? formVm.Pages.FirstOrDefault()
                : formVm.Pages.FirstOrDefault(m => m.PageId == pageId);

            return pageVm;
        }

    }
}