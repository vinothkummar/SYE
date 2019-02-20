using System.Collections.Generic;
using Newtonsoft.Json;

namespace GDSHelpers.Models.FormSchema
{
    public class FormVM
    {
        [JsonProperty("id")]
        public string Id { get; set; }


        [JsonProperty("form_name")]
        public string FormName { get; set; }


        [JsonProperty("pre_amble")]
        public string PreAmble { get; set; }


        [JsonProperty("last_modified")]
        public string LastModified { get; set; }


        [JsonProperty("pages")]
        public IEnumerable<PageVM> Pages { get; set; }

    }
}
