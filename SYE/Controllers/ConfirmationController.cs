using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SYE.Controllers
{
    public class ConfirmationController : Controller
    {

        [HttpGet, Route("form/you-have-sent-your-feedback")]
        public IActionResult Index(string id)
        {
            ViewBag.Reference = HttpContext.Session.GetString("ReferenceNumber");
            ViewBag.Title = "you have sent your feedback";
            return View();
        }
    }
}