using Microsoft.AspNetCore.Mvc;
using SYE.ViewModels;

namespace SYE.Controllers
{
    public class CookiesController : Controller
    {

        [HttpGet, Route("cookies")]
        public IActionResult Index()
        {
            //ViewBag.ShowBackButton = true;
            //ViewBag.PreviousPage = "javascript:history.go(-1);";

            ViewBag.BackLink = new BackLinkVM { Show = true, Url = "/", Text = "Home" };
            return View();
        }
    }
}