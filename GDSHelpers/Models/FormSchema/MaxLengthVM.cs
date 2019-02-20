using Newtonsoft.Json;

namespace GDSHelpers.Models.FormSchema
{
    public class MaxLengthVM
    {
        [JsonProperty("type")]
        public string Type { get; set; }


        [JsonProperty("max")]
        public int Max { get; set; }


        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

    }
}
