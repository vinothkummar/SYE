using Newtonsoft.Json;

namespace GDSHelpers.Models.FormSchema
{

    public class AnswerLogicVM
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("next_page_id")]
        public string NextPageId { get; set; }

    }
}
