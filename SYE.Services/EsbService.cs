using SYE.Models.SubmissionSchema;
using SYE.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SYE.Services
{
    public interface IEsbService
    {
        Task<IEnumerable<SubmissionVM>> GetAllSubmisions();
        Task<IEnumerable<SubmissionVM>> GetAllSubmisions(string status);
        Task<SubmissionVM> GetSubmision(string id);
        Task<bool> PostSubmision(SubmissionVM submission);
        Task<int> PostAllSubmisions();
    }
    public class EsbService : IEsbService
    {
        private readonly IGenericRepository<SubmissionVM> _repo;

        public EsbService(IGenericRepository<SubmissionVM> repo)
        {
            _repo = repo;
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

        public Task<bool> PostSubmision(SubmissionVM submission)
        {
            //throw new NotImplementedException();
            return Task.FromResult(true);
        }
        public Task<int> PostAllSubmisions()
        {
            throw new NotImplementedException();
        }
    }
}
