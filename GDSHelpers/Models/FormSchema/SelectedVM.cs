using Newtonsoft.Json;

namespace GDSHelpers.Models.FormSchema
{
    public class SelectedVM
    {
        [JsonProperty("min")]
        public int Min { get; set; }


        [JsonProperty("max")]
        public int Max { get; set; }


        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

    }
}
