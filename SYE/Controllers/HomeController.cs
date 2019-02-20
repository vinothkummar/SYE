using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SYE.Models;
using SYE.Services;

namespace SYE.Controllers
{
    public class HomeController : Controller
    {

        private readonly ISubmissionService _submissionService;

        public HomeController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

        public IActionResult Index()
        {
            _submissionService.FindByAsync(m => m.FormName == "");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
