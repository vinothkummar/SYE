using System;
using SYE.Models.SubmissionSchema;
using SYE.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SYE.EsbWrappers;
using System.Xml.Serialization;
using System.Web;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace SYE.Services
{
    public interface IEsbService
    {
        Task<IEnumerable<SubmissionVM>> GetAllSubmisions();
        Task<IEnumerable<SubmissionVM>> GetAllSubmisions(string status);
        Task<SubmissionVM> GetSubmision(string id);
        Task<string> PostSubmision(SubmissionVM submission);
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

        public async Task<string> PostSubmision(SubmissionVM submission)
        {
            var result = await _esbWrapper.PostSubmission(submission);
            if (!string.IsNullOrWhiteSpace(result))
            {
                submission.Status = "Posted";
                var sub = await _repo.UpdateAsync(submission.Id, submission);
            }

            return result;
        }
        private string GenerateEsbPayload(SubmissionVM payload)
        {
            string serialized;

            serialized = SerializePayload<SubmissionVM>(payload);

            return serialized;
        }

        private string SerializePayload<T>(SubmissionVM payload)
        {
            var serializer = new XmlSerializer(typeof(T));
            var writer = new Utf8StringWriter();
            serializer.Serialize(writer, payload);
            var escapedPayload = HttpUtility.HtmlEncode(writer.ToString());

            if (escapedPayload == null) throw new ArgumentException("Payload cannot be null.");
            var serialized = new XCData(escapedPayload);
            return serialized.ToString();
        }

    }
  
    internal class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }

}
