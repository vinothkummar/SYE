using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using SYE.Helpers.Extensions;

namespace SYE.Models.Response
{
    public class StatusResult : ActionResult
    {
        public StatusResult(int statusCode, string message, HttpContext context)
        {
            context.Features.Get<IHttpResponseFeature>().SetResponse(message, statusCode);
        }
    }
}
