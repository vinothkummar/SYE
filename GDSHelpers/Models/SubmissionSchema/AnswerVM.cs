using Newtonsoft.Json;

namespace GDSHelpers.Models.SubmissionSchema
{
    public class AnswerVM
    {
        [JsonProperty("question_id")]
        public string QuestionId { get; set; }


        [JsonProperty("question")]
        public string Question { get; set; }

        
        [JsonProperty("answer")]
        public string Answer { get; set; }
    }
}
