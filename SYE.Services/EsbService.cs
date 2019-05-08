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
        Task<List<SubmissionVM>> GetAllSubmisions();
        Task<List<SubmissionVM>> GetAllSubmisions(string status);
        Task<SubmissionVM> GetSubmision(string id);
        Task<bool> PostSubmision(string id);
    }
    public class EsbService : IEsbService
    {
        private readonly IGenericRepository<SubmissionVM> _repo;

        public EsbService(IGenericRepository<SubmissionVM> repo)
        {
            _repo = repo;
        }

        public Task<List<SubmissionVM>> GetAllSubmisions()
        {
            throw new NotImplementedException();
        }

        public Task<List<SubmissionVM>> GetAllSubmisions(string status)
        {
            //var results = _repo.FindByAsync(x => x.Status == status);
            //return results;
            throw new NotImplementedException();
        }

        public Task<SubmissionVM> GetSubmision(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PostSubmision(string id)
        {
            throw new NotImplementedException();
        }
    }
}
