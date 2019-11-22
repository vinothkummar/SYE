using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SYE.Helpers.Algorithm;
using SYE.Helpers.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SYE.MiddlewareExtensions
{
    public class KeyRequirementHandler : AuthorizationHandler<KeyRequirement>
    {
        public const string GFC_KEY_NAME = "id";
        public const int GFC_KEY = 0;
        public const int GFC_PASSWORD = 1;
        private readonly ILogger<KeyRequirementHandler> _logger;

        public KeyRequirementHandler(ILogger<KeyRequirementHandler> logger) => _logger = logger;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, KeyRequirement requirement)
        {            
            SucceedRequirementIfKeyPresentAndValid(context, requirement);
            return Task.CompletedTask;
        }

        private void SucceedRequirementIfKeyPresentAndValid(AuthorizationHandlerContext context, KeyRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext authorizationFilterContext)
            {
                if (!AllowCrossOrigin(authorizationFilterContext))
                {
                    _logger.LogError("Cross Domain Origin Resources Sharing Post Error Occured", EnumStatusCode.CQCIntegrationPayLoadNullError);
                    context.Fail();
                }


                //requested data using form-urlencoded
                var encryptedString = authorizationFilterContext.HttpContext.Request.Form[GFC_KEY_NAME].FirstOrDefault();

                if (encryptedString != null && requirement.Keys[GFC_PASSWORD] == AesAlgorithm.Decrypt(requirement.Keys[GFC_KEY], encryptedString))
                {
                    context.Succeed(requirement);
                }                
            }
        }        

        private Uri GetOrigin(AuthorizationFilterContext authorizationFilterContext)
        {
            var context = authorizationFilterContext.HttpContext.Request;
            Uri origin = null;

            var originHeader = context.Headers["Referer"].FirstOrDefault() ?? context.Headers["Host"].FirstOrDefault();
            if (!String.IsNullOrEmpty(originHeader) && Uri.TryCreate(originHeader, UriKind.Absolute, out origin))
                return origin;
            return null;
        }

        private bool IsOriginAllowed(Uri origin)
        {
            string[] allowedDomains = new[] { "www.cqc.org.uk", "dev.cqc.org.uk", "localhost"};

            if (allowedDomains.Contains(origin.Host))
            {
                return true;
            };

            return false;
        }

        private bool AllowCrossOrigin(AuthorizationFilterContext authorizationFilterContext)
        {
            
            Uri origin = GetOrigin(authorizationFilterContext);

            var response = authorizationFilterContext.HttpContext.Response;

            if (origin != null && IsOriginAllowed(origin))
            {
                // If the origin is allowed, add the specific header to the response
                response.Headers.Add("Access-Control-Allow-Origin", $"{origin.Scheme}://{origin.Host}");
                response.Headers.Add("Access-Control-Allow-Headers", new[] { "Origin, X-Requested-With, Content-Type, Accept" });
                response.Headers.Add("Access-Control-Allow-Methods", new[] { "POST, GET, OPTIONS" }); // new[] { "GET, POST, PUT, DELETE, OPTIONS" }
                //response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });
                return true;
            }

            return false;
        }
    }
}
