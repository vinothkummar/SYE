using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SYE.Models.SubmissionSchema;
using SYE.Repository;

namespace SYE.Services
{
    public interface ISubmissionService
    {
        Task<Document> CreateAsync(SubmissionVM item);
        Task DeleteAsync(string id);
        Task<SubmissionVM> GetByIdAsync(string id);
        Task<IEnumerable<SubmissionVM>> FindByAsync(Expression<Func<SubmissionVM, bool>> predicate);
        Task<Document> UpdateAsync(string id, SubmissionVM item);
        Task<int> GenerateUniqueUserRefAsync();
    }

    public class SubmissionService : ISubmissionService
    {
        private readonly IGenericRepository<SubmissionVM> _repo;
        private readonly IGenericRepository<ConfigVM> _config;
        private readonly IAppConfiguration<ConfigVM> _appConfig;

        public SubmissionService(IGenericRepository<SubmissionVM> repo, IGenericRepository<ConfigVM> config, IAppConfiguration<ConfigVM> appConfig)
        {
            _repo = repo;
            _config = config;
            _appConfig = appConfig;
        }
        
        public Task<Document> CreateAsync(SubmissionVM item)
        {
            return _repo.CreateAsync(item);
        }

        public Task DeleteAsync(string id)
        {
            return _repo.DeleteAsync(id);
        }

        public Task<SubmissionVM> GetByIdAsync(string id)
        {
            return _repo.GetByIdAsync(id);
        }

        public Task<IEnumerable<SubmissionVM>> FindByAsync(Expression<Func<SubmissionVM, bool>> predicate)
        {
            return _repo.FindByAsync(predicate);
        }

        public Task<Document> UpdateAsync(string id, SubmissionVM item)
        {
            return _repo.UpdateAsync(id, item);
        }

        public Task<int> GenerateUniqueUserRefAsync()
        {
            var configVm = _config.GetAsync(x => x.Id == _appConfig.ConfigRecordId, null, x => x.LastGeneratedRef).Result;

            var submissionId = int.Parse(configVm.LastGeneratedRef) + 1;
            configVm.LastGeneratedRef = submissionId.ToString();
            var result = _config.UpdateAsync(_appConfig.ConfigRecordId, configVm);

            return Task.FromResult(submissionId);
        }
    }
}
