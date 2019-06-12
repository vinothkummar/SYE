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

    //TODO secure this API!! Temporarity commented out
    [Route("api/[controller]")]
    [ApiController]
    public class EsbController : ControllerBase
    {
        private IEsbService _esbService;

        public EsbController(IEsbService esbService)
        {
            _esbService = esbService;
        }

        [HttpGet("submissions")]
        public async Task<ActionResult<IEnumerable<SubmissionVM>>> GetAll()
        {
            try
            {
                var results = await _esbService.GetAllSubmisions();
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
        [HttpGet("submissions/{status}")]
        public async Task<ActionResult<List<SubmissionVM>>> GetAll(string status)
        {
            try
            {
                var results = await _esbService.GetAllSubmisions(status);
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
        [HttpGet("submission/{id}")]
        public async Task<ActionResult<SubmissionVM>> Get(string id)
        {
            try
            {
                var result = await _esbService.GetSubmision(id);
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
        [HttpPost("submission")]
        public ActionResult<SubmissionPostResultVM> PostToCrm([FromBody] string id)
        {
            try
            {
                var submission = _esbService.GetSubmision(id).Result;
                if (submission == null)
                {
                    return NotFound();
                }

                var result = GeneratePostsToCrm(new List<string> { submission.SubmissionId });
                return Ok(result);
            }
            catch (Exception e)
            {
                //log error
                return StatusCode(500);
            }
        }
        [HttpPost("submissions")]
        public async Task<ActionResult<SubmissionPostResultVM>> PostAllToCrm()
        {
            try
            {
                var allIds = _esbService.GetAllSubmisions("Saved").Result.Select(x => x.SubmissionId);
                var result = await GeneratePostsToCrm(allIds);
                return Ok(result);
            }
            catch (Exception e)
            {
                //log error
                return StatusCode(500);
            }
        }

        private async Task<SubmissionPostResultVM> GeneratePostsToCrm(IEnumerable<string> ids)
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
                    var result = await _esbService.PostSubmision(submission);
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