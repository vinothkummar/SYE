using Microsoft.AspNetCore.Mvc;

namespace SYE.Controllers
{
    public class CheckYourAnswersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}