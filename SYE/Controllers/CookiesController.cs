using Microsoft.AspNetCore.Mvc;
using SYE.ViewModels;

namespace SYE.Controllers
{
    public class CookiesController : Controller
    {

        [HttpGet, Route("cookies")]
        public IActionResult Index()
        {
            ViewBag.BackLink = new BackLinkVM { Show = true, Url = "/", Text = "Home" };
            return View();
        }
    }
}