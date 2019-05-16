using SYE.Models.SubmissionSchema;
using SYE.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SYE.EsbWrappers;

namespace SYE.Services
{
    public interface IEsbService
    {
        Task<IEnumerable<SubmissionVM>> GetAllSubmisions();
        Task<IEnumerable<SubmissionVM>> GetAllSubmisions(string status);
        Task<SubmissionVM> GetSubmision(string id);
        Task<bool> PostSubmision(SubmissionVM submission);
    }
    public class EsbService : IEsbService
    {
        private readonly IGenericRepository<SubmissionVM> _repo;
        private IEsbWrapper _esbWrapper;

        public EsbService(IGenericRepository<SubmissionVM> repo, IEsbWrapper esbWrapper)
        {
            _repo = repo;
            _esbWrapper = esbWrapper;
        }

        public async Task<IEnumerable<SubmissionVM>> GetAllSubmisions()
        {
            var results = await _repo.FindByAsync(x => x.SubmissionId != "");
            return results;
        }

        public async Task<IEnumerable<SubmissionVM>> GetAllSubmisions(string status)
        {
            var results = await _repo.FindByAsync(x => x.Status == status);
            return results;
        }

        public async Task<SubmissionVM> GetSubmision(string id)
        {
            var result = await _repo.GetAsync(x => x.SubmissionId == id, null, x => x.SubmissionId);
            return result;
        }

        public async Task<bool> PostSubmision(SubmissionVM submission)
        {
            var json = JsonConvert.SerializeObject(submission);
            var result = await _esbWrapper.PostSubmission(json);
            if (result == true)
            {
                submission.Status = "Posted";
                var sub = await _repo.UpdateAsync(submission.Id, submission);
                return true;
            }

            return false;
        }
    }
}
