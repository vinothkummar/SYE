using SYE.Models.SubmissionSchema;
using System;
using System.Threading.Tasks;

namespace SYE.EsbWrappers
{
    public interface IEsbWrapper
    {
        Task<bool> PostSubmission(SubmissionVM submission);
    }
    public class EsbWrapper : IEsbWrapper
    {
        private IEsbClient _client;
        public EsbWrapper(IEsbClient client)
        {
            _client = client;
        }
        public async Task<bool> PostSubmission(SubmissionVM submission)
        {
            var returnBool = false;
            try
            {
                var result = _client.SendGenericAttachment(submission, PayloadType.Classified);
                returnBool = (!string.IsNullOrWhiteSpace(result));
            }
            catch (Exception e)
            {
                //log error           
            }

            return await Task.FromResult(returnBool);
        }
    }
}
