using System;
using System.Xml.Schema;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SYE.Models;

namespace SYE.Services
{
    public interface ISessionService
    {
        void SetSessionVars(SessionVM vm);
        void SaveFormVmToSession(FormVM vm);
        FormVM GetFormVmFromSession();
    }

    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _context;
        const string schemaKey = "sye_form_schema";

        public SessionService(IHttpContextAccessor context)
        {
            _context = context;
        }

        public void SetSessionVars(SessionVM vm)
        {
            var context = _context.HttpContext;
            context.Session.SetString("ProviderId", vm.ProviderId);
            context.Session.SetString("LocationId", vm.LocationId);
            context.Session.SetString("LocationName", vm.LocationName);
        }

        public void SaveFormVmToSession(FormVM vm)
        {
            var context = _context.HttpContext;
            context.Session.SetString(schemaKey, JsonConvert.SerializeObject(vm));
        }

        public FormVM GetFormVmFromSession()
        {
            var context = _context.HttpContext;
            context.Session.TryGetValue(schemaKey, out var value);
            return value == null ? default(FormVM) : JsonConvert.DeserializeObject<FormVM>(BitConverter.ToString(value));
        }

    }

}
