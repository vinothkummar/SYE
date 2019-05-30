using Microsoft.AspNetCore.Mvc;

namespace SYE.Controllers
{
    public class CookiesController : Controller
    {

        [HttpGet, Route("cookies")]
        public IActionResult Index()
        {
            return View();
        }
    }
}