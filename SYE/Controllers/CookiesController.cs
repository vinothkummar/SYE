using Microsoft.AspNetCore.Mvc;

namespace SYE.Controllers
{
    public class CookiesController : Controller
    {

        [HttpGet, Route("cookies")]
        public IActionResult Index()
        {
            ViewBag.ShowBackButton = true;
            ViewBag.PreviousPage = "javascript:history.go(-1);";
            return View();
        }
    }
}