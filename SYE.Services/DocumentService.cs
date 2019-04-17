using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SYE.Services
{
    public interface IDocumentService
    {
        Task<bool> CreateDocumentFromJson(string json);
    }
    public class DocumentService : IDocumentService
    {
        public Task<bool> CreateDocumentFromJson(string json)
        {
            throw new NotImplementedException();
        }
    }
}
