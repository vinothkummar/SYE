using SYE.Models.SubmissionSchema;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SYE.EsbWrappers
{
    public interface IEsbWrapper
    {
        Task<bool> PostSubmission(string submissionJson);
    }
    public class EsbWrapper : IEsbWrapper
    {
        public async Task<bool> PostSubmission(string submissionJson)
        {
            var returnBool = false;
            try
            {
                var result = EsbClient.SendGenericAttachment("", EsbClient.PayloadType.Submission);
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
