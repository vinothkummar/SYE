using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using SYE.Models;
using SYE.Helpers.Algorithm;

namespace SYE.MiddlewareExtensions
{
    public class KeyRequirementHandler : AuthorizationHandler<KeyRequirement>
    {
        public const string GFC_KEY_NAME = "GFC-KEY";
        public const int GFC_KEY = 0;
        public const int GFC_PASSWORD = 1;

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, KeyRequirement requirement)
        {
            SucceedRequirementIfKeyPresentAndValid(context, requirement);
            return Task.CompletedTask;
        }

        private void SucceedRequirementIfKeyPresentAndValid(AuthorizationHandlerContext context, KeyRequirement requirement)
        {
            if (context.Resource is AuthorizationFilterContext authorizationFilterContext)
            {               
                var encryptedString = authorizationFilterContext.HttpContext.Request.Form[GFC_KEY_NAME].FirstOrDefault();                              

                if (encryptedString != null && requirement.Keys[GFC_PASSWORD] == AesAlgorithm.Decrypt(requirement.Keys[GFC_KEY], encryptedString))
                {
                    context.Succeed(requirement);
                }               
            }
        }
    }
}
