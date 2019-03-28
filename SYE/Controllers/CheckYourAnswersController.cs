using Microsoft.AspNetCore.Mvc;
using SYE.Models;
using SYE.Services;

namespace SYE.Controllers
{
    public class CheckYourAnswersController : Controller
    {
        private readonly ISessionService _sessionService;

        public CheckYourAnswersController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var vm = new CheckYourAnswersVm
            {
                FormVm = _sessionService.GetFormVmFromSession()
            };

            return View(vm);
        }


        [HttpPost]
        public IActionResult Index(CheckYourAnswersVm vm)
        {
            if (vm.SendConfirmationEmail)
            {
                //TODO: Send the confirmation email
            }


            //TODO: Save our submission to the DB and generate an id
            var reference = "456";


            return RedirectToAction("Index", "Confirmation", new { id = reference });
        }


    }
}