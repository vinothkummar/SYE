//using SYE.Models.SubmissionSchema;
//using System;
//using System.Threading.Tasks;

//namespace SYE.EsbWrappers
//{
//    public interface IEsbWrapper
//    {
//        Task<string> PostSubmission(SubmissionVM submission);
//    }
//    public class EsbWrapper : IEsbWrapper
//    {
//        private IEsbClient _client;
//        public EsbWrapper(IEsbClient client)
//        {
//            _client = client;
//        }
//        public async Task<string> PostSubmission(SubmissionVM submission)
//        {
//            var result = _client.SendGenericAttachment(submission, PayloadType.Classified);
//            return await Task.FromResult(result);
//        }
//    }
//}
