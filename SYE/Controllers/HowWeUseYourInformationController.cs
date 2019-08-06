using Microsoft.AspNetCore.Mvc;
using SYE.ViewModels;

namespace SYE.Controllers
{
    public class HowWeUseYourInformationController : Controller
    {
        [Route("how-we-handle-information")]
        public IActionResult Index()
        {
            ViewBag.BackLink = new BackLinkVM { Show = true, Url = "javascript:history.go(-1);", Text = "Back" };
            ViewBag.Title = "How we handle information - Give feedback on care";
            return View();
        }
    }
}