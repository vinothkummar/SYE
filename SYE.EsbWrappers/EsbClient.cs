using SYE.Models.SubmissionSchema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SYE.EsbWrappers
{
    public interface IEsbClient
    {
        Task<string> GetAuthentiacationKey();
        Task<bool> PostSubmission(SubmissionVM submission);
    }
    public class EsbClient : IEsbClient
    {
        public async Task<string> GetAuthentiacationKey()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> PostSubmission(SubmissionVM submission)
        {
            throw new NotImplementedException();
        }
    }
}
