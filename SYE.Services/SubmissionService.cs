using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using SYE.Repository;

namespace SYE.Services
{

    public interface ISubmissionService
    {
        Task<Document> GetItemByIdAsync(string id);

        Task<Document> CreateItemAsync(Document item);
    }

    public class SubmissionService : ISubmissionService
    {
        private readonly IGenericRepository<Document> _repository;

        public SubmissionService(IGenericRepository<Document> repository)
        {
            _repository = repository;
        }

        public Task<Document> GetItemByIdAsync(string id)
        {
            return _repository.GetItemByIdAsync(id);
        }

        public Task<Document> CreateItemAsync(Document item)
        {
            return _repository.CreateItemAsync(item);
        }

    }
}
