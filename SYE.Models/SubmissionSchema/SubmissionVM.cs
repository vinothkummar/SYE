using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SYE.Models.SubmissionSchema
{
    public class SubmissionVM
    {
        public SubmissionVM()
        {
            Id = Guid.NewGuid().ToString();
            DateCreated = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        }


        [JsonProperty("id")]
        public string Id { get; set; }


        [JsonProperty("version")]
        public string Version { get; set; }


        [JsonProperty("form_name")]
        public string FormName { get; set; }
        

        [JsonProperty("date_created")]
        public string DateCreated { get; set; }


        [JsonProperty("provider_id")]
        public string ProviderId { get; set; }


        [JsonProperty("location_id")]
        public string LocationId { get; set; }


        [JsonProperty("location_name")]
        public string LocationName { get; set; }


        [JsonProperty("answers")]
        public IEnumerable<AnswerVM> Answers { get; set; }


        [JsonProperty("user_ref")]
        public string UserRef { get; set; }

        [JsonProperty("base64_attachment")]
        public string Base64Attachment { get; set; }

    }
}
