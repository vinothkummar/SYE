using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GDSHelpers.Models.FormSchema;
using Newtonsoft.Json;

namespace SYE.Services
{
    public interface IPageService
    {
        FormVM GetFormVm(Dictionary<string, string> replacements = null);
        PageVM GetPageById(string pageId, string path, string locationName = "");
        PageVM GetPageById(string pageId, string locationName = "");
        PageVM GetPageById(FormVM formVm, string pageId);
    }
    public class PageService : IPageService
    {
        public FormVM GetFormVm(Dictionary<string, string> replacements = null)
        {
            //TODO: Refactor this method to read from Cosmos Db
            var path = "Content/form-schema.json";

            FormVM formVm;
            using (var r = new StreamReader(path))
            {
                var file = r.ReadToEnd();

                if (replacements != null)
                {
                    foreach (var item in replacements)
                    {
                        file = file.Replace(item.Key, item.Value);
                    }
                }
                
                formVm = JsonConvert.DeserializeObject<FormVM>(file);
            }

            return formVm;
        }


        /// <summary>
        /// this method reads a json file from the folder and returns the next page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="path"></param>
        /// <param name="locationName"></param>
        /// <returns></returns>
        public PageVM GetPageById(string pageId, string path, string locationName = "")
        {
            FormVM formVm;
            using (var r = new StreamReader(path))
            {
                var file = r.ReadToEnd();

                file = file.Replace("!!location_name!!", locationName);

                formVm = JsonConvert.DeserializeObject<FormVM>(file);
            }

            var pageVm = string.IsNullOrEmpty(pageId)
                ? formVm.Pages.FirstOrDefault()
                : formVm.Pages.FirstOrDefault(m => m.PageId == pageId);

            return pageVm;

        }

        /// <summary>
        /// this method will be used to get the file from the cache before returning the appropriate page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="locationName"></param>
        /// <returns></returns>
        public PageVM GetPageById(string pageId, string locationName = "")
        {
            throw new NotImplementedException();
        }

        public PageVM GetPageById(FormVM formVm, string pageId)
        {
            var pageVm = string.IsNullOrEmpty(pageId)
                ? formVm.Pages.FirstOrDefault()
                : formVm.Pages.FirstOrDefault(m => m.PageId == pageId);

            return pageVm;

        }

    }
}
