using SYE.EsbWrappers;
using SYE.Models.SubmissionSchema;
using SYE.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        private IEsbClient _esbClient;

        public EsbService(IGenericRepository<SubmissionVM> repo, IEsbClient esbClient)
        {
            _repo = repo;
            _esbClient = esbClient;
        }

        public async Task<IEnumerable<SubmissionVM>> GetAllSubmisions()
        {
            var results = await _repo.FindByAsync(x => x.UserRef != "");
            return results;
        }

        public async Task<IEnumerable<SubmissionVM>> GetAllSubmisions(string status)
        {
            var results = await _repo.FindByAsync(x => x.Status == status);
            return results;
        }

        public async Task<SubmissionVM> GetSubmision(string id)
        {
            var result = await _repo.GetAsync(x => x.UserRef == id, null, x => x.UserRef);
            return result;
        }

        public async Task<bool> PostSubmision(SubmissionVM submission)
        {
            var result = await _esbClient.PostSubmission(submission);
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
