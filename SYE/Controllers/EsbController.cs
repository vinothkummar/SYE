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
        public ActionResult<IEnumerable<SubmissionVM>> GetAll()
        {
            try
            {
                var results = _esbService.GetAllSubmisions().Result;
                if (results == null)
                {
                    return NotFound();
                }

                return Ok(results);
            }
            catch (Exception e)
            {
                //log error
                return StatusCode(500);
            }
        }
        [HttpGet]
        [Route("submissions/{status}")]
        public ActionResult<List<SubmissionVM>> GetAll(string status)
        {
            try
            {
                var results = _esbService.GetAllSubmisions(status).Result;
                if (results == null)
                {
                    return NotFound();
                }

                return Ok(results);
            }
            catch (Exception e)
            {
                //log error
                return StatusCode(500);
            }
        }
        [HttpGet]
        [Route("submission/{id}")]
        public ActionResult<SubmissionVM> Get(string id)
        {
            try
            {
                var result = _esbService.GetSubmision(id).Result;
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception e)
            {
                //log error
                return StatusCode(500);
            }
        }
        [HttpPost]
        [Route("submission")]
        public ActionResult<SubmissionPostResultVM> PostToCrm([FromBody] string id)
        {
            try
            {
                var submission = _esbService.GetSubmision(id).Result;
                if (submission == null)
                {
                    return NotFound();
                }

                var result = GeneratePostsToCrm(new List<string> {submission.UserRef});
                return Ok(result);
            }
            catch (Exception e)
            {
                //log error
                return StatusCode(500);
            }
        }
        [HttpPost]
        [Route("submissions")]
        public ActionResult<SubmissionPostResultVM> PostAllToCrm()
        {
            try
            {
                var allIds = _esbService.GetAllSubmisions("Saved").Result.Select(x => x.UserRef);
                var result = GeneratePostsToCrm(allIds);
                return Ok(result);
            }
            catch (Exception e)
            {
                //log error
                return StatusCode(500);
            }
        }

        private SubmissionPostResultVM GeneratePostsToCrm(IEnumerable<string> ids)
        {
            var submissionResult = new SubmissionPostResultVM();
            submissionResult.DateCreated = DateTime.Now.ToLongDateString();
            submissionResult.NumberItemsSent = ids.Count();
            foreach (var id in ids)
            {
                var submission = _esbService.GetSubmision(id).Result;
                if (submission == null)
                {
                    submissionResult.NumberItemsFailed++;
                }
                else
                {
                    var result = _esbService.PostSubmision(submission).Result;
                    if (result == true)
                    {
                        submissionResult.NumberItemsPosted++;
                    }
                    else
                    {
                        submissionResult.NumberItemsFailed++;
                    }
                }
            }

            if (submissionResult.NumberItemsFailed == 0 && submissionResult.NumberItemsPosted > 0)
            {
                submissionResult.Status = "Posted";
            }
            else
            {
                submissionResult.Status = "Failed";
            }
            return submissionResult;
        }        
    }
}