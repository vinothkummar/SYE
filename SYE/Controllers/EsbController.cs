using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SYE.Models.SubmissionSchema;
using SYE.Services;

namespace SYE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EsbController : ControllerBase
    {
        private IEsbService _esbService;

        public EsbController(IEsbService esbService)
        {
            _esbService = esbService;
        }
        [HttpGet]
        [Route("submissions")]
        public ActionResult<List<SubmissionVM>> GetAll()
        {
           _esbService.GetSubmisions()
        }
        [HttpGet]
        [Route("submissions/{status}")]
        public ActionResult<List<SubmissionVM>> GetAll(string status)
        {
            return null;
        }
        [HttpGet]
        [Route("submission/{id}")]
        public ActionResult<SubmissionVM> Get(string id)
        {
            return null;
        }
        [HttpPost]
        [Route("submission{id}")]
        public ActionResult<SubmissionVM> PostToCrm(string id)
        {
            return null;
        }
    }
}