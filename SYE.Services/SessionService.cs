using System.Collections.Generic;
using System.Linq;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SYE.Models;

namespace SYE.Services
{
    public interface ISessionService
    {
        void UpdateNavOrder(string currentPage);
        List<string> GetNavOrder();
        PageVM GetPageById(string pageId, bool notFoundFlag);
        FormVM LoadLatestFormIntoSession(Dictionary<string, string> replacements);
        void SetUserSessionVars(UserSessionVM vm);
        UserSessionVM GetUserSession();
        void SaveFormVmToSession(FormVM vm);
        FormVM GetFormVmFromSession();
        void UpdatePageVmInFormVm(PageVM vm);
        void SaveUserSearch(string search);
        string GetUserSearch();
        void ClearSession();
    }

    public class SessionService : ISessionService
    {
        private readonly IFormService _formService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        const string schemaKey = "sye_form_schema";

        public SessionService(IFormService formService, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _formService = formService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }


        public void UpdateNavOrder(string currentPage)
        {
            var userSession = GetUserSession();

            if (userSession.NavOrder == null)
            {
                //First page
                userSession.NavOrder = new List<string>{ currentPage };
            }
            else
            {
                //If we've not been here before add the page
                if (!userSession.NavOrder.Contains(currentPage))
                {
                    userSession.NavOrder.Add(currentPage);
                }
                else
                {
                    //We have been here do delete everything after, just in case it changes
                    var newNav = new List<string>();
                   
                    var index = userSession.NavOrder.IndexOf(currentPage);
                    foreach (var page in userSession.NavOrder)
                    {
                        if (userSession.NavOrder.IndexOf(page) <= index) newNav.Add(page);
                    }

                    //Update the users navigation history
                    userSession.NavOrder = newNav;
                }
            }

            SetUserSessionVars(userSession);
        }

        public List<string> GetNavOrder()
        {
            var userSession = GetUserSession();
            return userSession.NavOrder ?? new List<string>();
        }


        public PageVM GetPageById(string pageId, bool notFoundFlag)
        {
            var formVm = GetFormVmFromSession();

            if (string.IsNullOrWhiteSpace(pageId))
            {
                return notFoundFlag
                    ? formVm.Pages.FirstOrDefault()
                    : formVm.Pages.FirstOrDefault(x => x.PageId != formVm.Pages.First().PageId);
            }

            return formVm.Pages.Any(x => x.PageId == pageId)
                ? formVm.Pages.FirstOrDefault(m => m.PageId == pageId)
                : null;
        }

        public FormVM LoadLatestFormIntoSession(Dictionary<string, string> replacements)
        {
            string formName = _configuration.GetSection("FormsConfiguration:ServiceForm").GetValue<string>("Name");
            string version = _configuration.GetSection("FormsConfiguration:ServiceForm").GetValue<string>("Version");
            var context = _httpContextAccessor.HttpContext;
            string sessionVersion = context.Session.GetString("FormVersion");

            if (!string.IsNullOrWhiteSpace(sessionVersion))
            {
                version = sessionVersion;
            }

            var form = string.IsNullOrEmpty(version) ?
                _formService.GetLatestFormByName(formName).Result :
                _formService.FindByNameAndVersion(formName, version).Result;

            var json = JsonConvert.SerializeObject(form);

            if (replacements?.Count > 0)
            {
                foreach (var item in replacements)
                {
                    json = json.Replace(item.Key, item.Value);
                }
            }

            var formVm = JsonConvert.DeserializeObject<FormVM>(json);
            SaveFormVmToSession(formVm);

            return formVm;
        }

        public void SetUserSessionVars(UserSessionVM vm)
        {
            var context = _httpContextAccessor.HttpContext;
            context.Session.SetString("ProviderId", vm.ProviderId ?? "");
            context.Session.SetString("LocationId", vm.LocationId ?? "");
            context.Session.SetString("LocationName", vm.LocationName ?? "");

            if (vm.NavOrder != null)
            {
                var pageList = string.Join(",", vm.NavOrder.ToArray<string>());
                context.Session.SetString("NavOrder", pageList ?? "");
            }
        }

        public UserSessionVM GetUserSession()
        {
            var context = _httpContextAccessor.HttpContext;
            var userSessionVm = new UserSessionVM
            {
                ProviderId = context.Session.GetString("ProviderId"),
                LocationId = context.Session.GetString("LocationId"),
                LocationName = context.Session.GetString("LocationName")
            };

            if (!string.IsNullOrEmpty(context.Session.GetString("NavOrder")))
                userSessionVm.NavOrder = context.Session.GetString("NavOrder").Split(',').ToList();

            return userSessionVm;
        }

        public void SaveFormVmToSession(FormVM vm)
        {
            var context = _httpContextAccessor.HttpContext;
            context.Session.SetString(schemaKey, JsonConvert.SerializeObject(vm));
        }

        public FormVM GetFormVmFromSession()
        {
            var context = _httpContextAccessor.HttpContext;
            var json = context.Session.GetString(schemaKey);
            return json == null ? default(FormVM) : JsonConvert.DeserializeObject<FormVM>(json);
        }

        public void UpdatePageVmInFormVm(PageVM vm)
        {
            var formVm = GetFormVmFromSession();
            var currentPage = formVm.Pages.FirstOrDefault(m => m.PageId == vm.PageId)?.Questions;
            foreach (var question in vm.Questions)
            {
                var q = currentPage.FirstOrDefault(m => m.QuestionId == question.QuestionId);
                q.Answer = question.Answer;
            }
            SaveFormVmToSession(formVm);
        }

        public void SaveUserSearch(string search)
        {
            var context = _httpContextAccessor.HttpContext;
            context.Session.SetString("Search", search);
        }

        public string GetUserSearch()
        {
            var context = _httpContextAccessor.HttpContext;
            return context.Session.GetString("Search");
        }

        public void ClearSession()
        {
            var context = _httpContextAccessor.HttpContext;
            context.Session.Clear();
        }
    }

}
