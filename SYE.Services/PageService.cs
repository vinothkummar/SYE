using System;
using System.Linq;
using System.Threading.Tasks;
using GDSHelpers.Models.FormSchema;
using SYE.Repository;

namespace SYE.Services
{
    public interface IPageService
    {
        [Obsolete("Please remove all references to this function as this will be removed in next sprint.")]
        Task<FormVM> GetLatestForm();

        [Obsolete("Please remove all references to this function as this will be removed in next sprint.")]
        Task<FormVM> GetFormById(string id);
    }
    public class PageService : IPageService
    {
        private readonly IGenericRepository<FormVM> _repo;

        public PageService(IGenericRepository<FormVM> repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// This method always returns latest form schema from Cosmos DB based on last modified date stored in form schema document.
        /// </summary>
        /// <returns></returns>
        public Task<FormVM> GetLatestForm()
        {
            return _repo.GetAsync(null, null, (x => x.LastModified));
        }

        public Task<FormVM> GetFormById(string id)
        {
            return _repo.GetByIdAsync(id);            
        }

        ///// <summary>
        ///// this method reads a json file from the folder and returns the next page
        ///// </summary>
        ///// <param name="pageId"></param>
        ///// <param name="path"></param>
        ///// <param name="locationName"></param>
        ///// <returns></returns>
        ///// <remarks>
        ///// Please refactor this function (and all tests consuming this method) so method accepts whole form schema and returns required page.
        ///// If we need to load form from database/cache/session/file-system it has to be done as a seperate function
        ///// </remarks>
        //[Obsolete("Please remove all references to this function as this will be removed in next sprint.")]
        //public PageVM GetPageById(string pageId, string path, string locationName = "")
        //{
        //    var file = string.Empty;

        //    if (string.IsNullOrWhiteSpace(path))
        //    {
        //        throw new ArgumentException(nameof(path));
        //    }

        //    using (var r = new StreamReader(path))
        //    {
        //        file = r.ReadToEnd();
        //    }

        //    file = file.Replace("!!location_name!!", locationName);
        //    var formVm = JsonConvert.DeserializeObject<FormVM>(file);

        //    if (string.IsNullOrWhiteSpace(pageId))
        //    {
        //        return formVm.Pages.FirstOrDefault();
        //    }

        //    if (formVm.Pages.Any(x => x.PageId == pageId))
        //    {
        //        return formVm.Pages.FirstOrDefault(m => m.PageId == pageId);
        //    }

        //    return null;
        //}

    }
}
