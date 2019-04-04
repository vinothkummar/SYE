﻿using System.Collections.Generic;
using System.Linq;
using GDSHelpers.Models.FormSchema;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SYE.Models;
using SYE.Repository;

namespace SYE.Services
{
    public interface ISessionService
    {
        PageVM GetPageById(string pageId);
        FormVM LoadLatestFormIntoSession(Dictionary<string, string> replacements);
        void SetUserSessionVars(UserSessionVM vm);
        UserSessionVM GetUserSession();
        void SaveFormVmToSession(FormVM vm);
        FormVM GetFormVmFromSession();
        void UpdatePageVmInFormVm(PageVM vm);
        void SaveUserSearch(string search);
        string GetUserSearch();
    }

    public class SessionService : ISessionService
    {
        private readonly IGenericRepository<FormVM> _repo;
        private readonly IFormService _formService;
        private readonly IHttpContextAccessor _context;

        const string schemaKey = "sye_form_schema";

        public SessionService(IFormService formService, IHttpContextAccessor context)
        {
            _formService = formService;
            _context = context;
        }
        
        public PageVM GetPageById(string pageId)
        {
            var formVm = GetFormVmFromSession();

            if (string.IsNullOrWhiteSpace(pageId))
            {
                return formVm.Pages.FirstOrDefault();
            }

            if (formVm.Pages.Any(x => x.PageId == pageId))
            {
                return formVm.Pages.FirstOrDefault(m => m.PageId == pageId);
            }

            return null;
        }
        
        public FormVM LoadLatestFormIntoSession(Dictionary<string, string> replacements)
        {
            var context = _context.HttpContext;
            var version = context.Session.GetString("FormVersion");

            var form = string.IsNullOrEmpty(version) ? 
                _formService.GetLatestForm().Result : 
                _formService.FindByVersion(version).Result;

            var json = JsonConvert.SerializeObject(form);

            if (replacements != null && replacements.Count > 0)
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
            var context = _context.HttpContext;
            context.Session.SetString("ProviderId", vm.ProviderId ?? "");
            context.Session.SetString("LocationId", vm.LocationId ?? "");
            context.Session.SetString("LocationName", vm.LocationName ?? "");
        }

        public UserSessionVM GetUserSession()
        {
            var context = _context.HttpContext;
            var userSessionVm = new UserSessionVM
            {
                ProviderId = context.Session.GetString("ProviderId"),
                LocationId = context.Session.GetString("LocationId"),
                LocationName = context.Session.GetString("LocationName")
            };
            return userSessionVm;
        }



        public void SaveFormVmToSession(FormVM vm)
        {
            var context = _context.HttpContext;
            context.Session.SetString(schemaKey, JsonConvert.SerializeObject(vm));
        }

        public FormVM GetFormVmFromSession()
        {
            var context = _context.HttpContext;
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
            var context = _context.HttpContext;
            context.Session.SetString("Search", search);
        }

        public string GetUserSearch()
        {
            var context = _context.HttpContext;
            var search = context.Session.GetString("Search");
            return search;
        }
    }

}