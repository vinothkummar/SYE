using System.Linq;
using System.Threading.Tasks;
using GDSHelpers.Models.FormSchema;
using SYE.Repository;

namespace SYE.Services
{
    public interface IFormService
    {
        Task<FormVM> GetLatestForm();
        Task<FormVM> GetFormById(string id);
        Task<FormVM> FindByName(string formName);
    }

    public class FormService : IFormService
    {
        private readonly IGenericRepository<FormVM> _repo;

        public FormService(IGenericRepository<FormVM> repo)
        {
            _repo = repo;
        }

        public Task<FormVM> GetLatestForm()
        {
            return _repo.GetAsync(null, null, (x => x.LastModified));
        }

        public Task<FormVM> GetFormById(string id)
        {
            return _repo.GetByIdAsync(id);
        }

        public async Task<FormVM> FindByName(string formName)
        {
            return _repo.FindByAsync(m => m.FormName == formName).Result.FirstOrDefault();
        }

    }
}
