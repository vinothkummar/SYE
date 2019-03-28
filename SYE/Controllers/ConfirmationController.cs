using Microsoft.AspNetCore.Mvc;

namespace SYE.Controllers
{
    public class ConfirmationController : Controller
    {
        public IActionResult Index(string id)
        {
            return View();
        }
    }
}