﻿using Microsoft.AspNetCore.Mvc;

namespace SYE.Controllers
{
    public class HowWeUseYourInformationController : Controller
    {
        [Route("how-we-handle-information")]
        public IActionResult Index()
        {
            ViewBag.ShowBackButton = true;
            ViewBag.PreviousPage = "javascript:history.go(-1);";
            return View();
        }
    }
}