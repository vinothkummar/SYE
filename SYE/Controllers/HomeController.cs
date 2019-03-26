using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SYE.Models;
using SYE.Services;
using Microsoft.AspNetCore.Http;

namespace SYE.Controllers
{
    public class HomeController : Controller
    {

        private readonly ISubmissionService _submissionService;

        public HomeController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        public IActionResult Index(string formid = "")
        {
            _submissionService.FindByAsync(m => m.FormName == "");

            if (!string.IsNullOrWhiteSpace(formid))
            {
                HttpContext.Session.SetString("_form_id_", formid);
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
