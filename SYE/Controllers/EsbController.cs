using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SYE.Models.SubmissionSchema;
using SYE.Services;

namespace SYE.Controllers
{

    //TODO secure this API!! Temporarity commented out
    [Route("api/[controller]")]
    [ApiController]
    public class EsbController : ControllerBase
    {
        private readonly ILogger _logger;
        private IEsbService _esbService;

        public EsbController(IEsbService esbService, ILogger<EsbController> logger)
        {
            _esbService = esbService;
            _logger = logger;
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting submissions.");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting submissions.");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting submission.");
                return StatusCode(500);
            }
        }
        [HttpPost("submission")]
        public async Task<ActionResult<SubmissionPostResultVM>> PostToCrm([FromBody] string id)
        {
            try
            {
                var submission = _esbService.GetSubmision(id).Result;
                if (submission == null)
                {
                    return NotFound();
                }
                if (submission.Status != "Saved")
                {
                    return NotFound();
                }

                var result = await GeneratePostsToCrm(new List<string> { submission.SubmissionId });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting submission to CRM.");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error posting submissions to CRM.");
                return StatusCode(500);
            }
        }

        private async Task<SubmissionPostResultVM> GeneratePostsToCrm(IEnumerable<string> ids)
        {
            var submissionResult = new SubmissionPostResultVM
            {
                DateCreated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
                NumberItemsSent = ids.Count(),
                PostedSubmissions = new List<string>(),
                FailedSubmissions = new List<string>()
            };
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
                    if (!string.IsNullOrWhiteSpace(result))
                    {
                        submissionResult.NumberItemsPosted++;
                        submissionResult.PostedSubmissions.Add(result);//add the enquiryId returned from the esb
                    }
                    else
                    {
                        submissionResult.NumberItemsFailed++;
                        submissionResult.FailedSubmissions.Add("GFC-" + id);//add the id of the submisison that we are tring to post
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