using System;
using System.Threading.Tasks;

namespace SYE.EsbWrappers
{
    public interface IEsbWrapper
    {
        Task<bool> PostSubmission(string submissionJson);
    }
    public class EsbWrapper : IEsbWrapper
    {
        private IEsbClient _client;
        public EsbWrapper(IEsbClient client)
        {
            _client = client;
        }
        public async Task<bool> PostSubmission(string submissionJson)
        {
            var returnBool = false;
            try
            {
                var result = _client.SendGenericAttachment(submissionJson, PayloadType.Submission);
                returnBool = true;//TODO look at the result before determining true/false
            }
            catch (Exception e)
            {
                //log error           
            }


            return await Task.FromResult(returnBool);
        }
    }
}
