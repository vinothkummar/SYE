using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SYE.MiddlewareExtensions
{
    public class KeyRequirement : IAuthorizationRequirement
    {
        public IReadOnlyList<string> Keys { get; set; }

        public KeyRequirement(IEnumerable<string> keys)
        {
            Keys = keys?.ToList() ?? new List<string>();
        }
    }
}
