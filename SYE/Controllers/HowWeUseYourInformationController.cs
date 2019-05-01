using Microsoft.AspNetCore.Mvc;

namespace SYE.Controllers
{
    public class HowWeUseYourInformationController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.ShowBackButton = true;
            ViewBag.PreviousPage = "/";
            return View();
        }
    }
}