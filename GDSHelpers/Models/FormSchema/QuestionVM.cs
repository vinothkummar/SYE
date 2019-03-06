using System.Collections.Generic;
using Newtonsoft.Json;

namespace GDSHelpers.Models.FormSchema
{
    public class QuestionVM
    {
        [JsonProperty("question_id")]
        public string QuestionId { get; set; }


        [JsonProperty("question")]
        public string Question { get; set; }


        [JsonProperty("additional_text")]
        public string AdditionalText { get; set; }


        [JsonProperty("data_type")]
        public string DataType { get; set; }


        [JsonProperty("input_type")]
        public string InputType { get; set; }


        [JsonProperty("options")]
        public string Options { get; set; }

        [JsonProperty("answer_logic")]
        public IEnumerable<AnswerLogicVM> AnswerLogic { get; set; }


        [JsonProperty("show_when")]
        public ShowWhenVM ShowWhen { get; set; }


        [JsonProperty("validation")]
        public ValidationVM Validation { get; set; }


        [JsonProperty("answer")]
        public string Answer { get; set; }
    }
}
