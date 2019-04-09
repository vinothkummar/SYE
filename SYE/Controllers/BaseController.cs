using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SYE.Services;

namespace SYE.Controllers
{
    public abstract class BaseController<T> : Controller where T : BaseController<T>
    {

        ILogger _logger;
        protected ILogger Logger => _logger ?? (_logger = ControllerContext.HttpContext.RequestServices.GetService<ILogger<T>>());

        ISessionService _sessionService;
        protected ISessionService SessionService => _sessionService ?? (_sessionService = ControllerContext.HttpContext.RequestServices.GetService<ISessionService>());

        public BaseController(IHttpContextAccessor context)
        {
            _logger = context?.HttpContext?.RequestServices?.GetService<ILogger<T>>() as ILogger;
            _sessionService = context?.HttpContext?.RequestServices?.GetService<ISessionService>() ?? null;
        }
    }
}