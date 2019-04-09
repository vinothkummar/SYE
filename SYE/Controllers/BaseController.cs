using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SYE.Services;

namespace SYE.Controllers
{
    public abstract class BaseController<T> : Controller where T : BaseController<T>
    {
        protected ILogger Logger { get; }

        protected ISessionService SessionService { get; }

        protected BaseController(IHttpContextAccessor context)
        {
            this.Logger = context?.HttpContext?.RequestServices?.GetService<ILogger<T>>() as ILogger;
            this.SessionService = context?.HttpContext?.RequestServices?.GetService<ISessionService>() ?? null;
        }
    }
}