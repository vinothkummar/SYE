using Microsoft.AspNetCore.Mvc;

namespace SYE.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}