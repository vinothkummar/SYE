using Microsoft.AspNetCore.Mvc;

namespace SYE.Controllers
{
    public class SearchController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Index(string id)
        {
            ViewBag.ShowResults = true;
            return View();
        }

    }
}