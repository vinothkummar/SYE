using Newtonsoft.Json;

namespace GDSHelpers.Models.FormSchema
{
    public class RequiredVM
    {
        [JsonProperty("is_required")]
        public bool IsRequired { get; set; }


        [JsonProperty("error_message")]
        public string ErrorMessage { get; set; }

    }
}
