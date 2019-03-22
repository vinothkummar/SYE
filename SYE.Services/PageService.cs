using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GDSHelpers.Models.FormSchema;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace SYE.Services
{
    public interface IPageService
    {
        PageVM GetPageById(string pageId, string path, string locationName = "");
    }
    public class PageService : IPageService
    {
        private readonly IDistributedCache _cache;
        public PageService()
        {

        }

        public PageService(IDistributedCache cache)
        {
            _cache = cache;
        }

        //public PageService(IServiceProvider serviceProvider)
        //{
        //    _cache = (serviceProvider.GetService(typeof(IDistributedCache)) as IDistributedCache);
        //}

        /// <summary>
        /// this method reads a json file from the folder and returns the next page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="path"></param>
        /// <param name="locationName"></param>
        /// <returns></returns>
        /// <remarks>
        /// Please note: This method contains temporary implementation to cache form schema in Redis to unblock developers testing this service
        /// and will be refactored to implament more robust caching functionality.
        /// The form schema is still loaded from file system and not from Cosmos DB (this will be available in next iteration).
        /// </remarks>
        public PageVM GetPageById(string pageId, string path, string locationName = "")
        {
            FormVM formVm = null;
            String file = String.Empty;
            byte[] encodedFormVM = _cache != null ? _cache.Get("_formVM_") ?? null : null;
            if (encodedFormVM != null)
            {
                file = Encoding.UTF8.GetString(encodedFormVM);
            }
            if (String.IsNullOrWhiteSpace(file))
            {
                using (var r = new StreamReader(path))
                {
                    file = r.ReadToEnd();
                }
                if (String.IsNullOrWhiteSpace(file) == false)
                {
                    file = file.Replace("!!location_name!!", locationName);
                }
            }
            if (String.IsNullOrWhiteSpace(file) == false)
            {
                if (encodedFormVM == null)
                {
                    encodedFormVM = Encoding.UTF8.GetBytes(file);
                    _cache.Set("_formVM_", encodedFormVM, new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30)));
                }
                formVm = JsonConvert.DeserializeObject<FormVM>(file);
            }

            var pageVm = string.IsNullOrEmpty(pageId)
                ? formVm.Pages.FirstOrDefault()
                : formVm.Pages.FirstOrDefault(m => m.PageId == pageId);

            return pageVm;

        }

    }
}
