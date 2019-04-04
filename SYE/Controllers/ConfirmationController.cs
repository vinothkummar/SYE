using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SYE.Controllers
{
    public class ConfirmationController : Controller
    {
        public IActionResult Index(string id)
        {
            ViewBag.Reference = HttpContext.Session.GetString("ReferenceNumber");

            return View();
        }
    }
}