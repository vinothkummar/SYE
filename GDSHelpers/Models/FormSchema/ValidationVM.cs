using Newtonsoft.Json;

namespace GDSHelpers.Models.FormSchema
{
    public class ValidationVM
    {
        [JsonProperty("required")]
        public RequiredVM Required { get; set; }


        [JsonProperty("max_length")]
        public MaxLengthVM MaxLength { get; set; }


        [JsonProperty("selected")]
        public SelectedVM Selected { get; set; }


        [JsonProperty("is_errored")]
        public bool IsErrored { get; set; }


        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

    }
}
