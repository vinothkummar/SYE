using Newtonsoft.Json;

namespace GDSHelpers.Models.FormSchema
{
    public class ButtonVM
    {
        [JsonProperty("button_text")]
        public string ButtonText { get; set; }


        [JsonProperty("button_type")]
        public string ButtonType { get; set; }

    }
}
