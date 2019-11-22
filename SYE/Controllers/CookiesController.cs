using Microsoft.AspNetCore.Mvc;
using SYE.ViewModels;

namespace SYE.Controllers
{
    public class CookiesController : Controller
    {

        [HttpGet, Route("cookies")]
        public IActionResult Index()
        {
            ViewData["HomeLink"] = false;
            return View();
        }
    }
}