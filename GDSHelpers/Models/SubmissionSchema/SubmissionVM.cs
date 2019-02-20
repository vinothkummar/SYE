using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GDSHelpers.Models.SubmissionSchema
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


        [JsonProperty("form_name")]
        public string FormName { get; set; }
        

        [JsonProperty("date_created")]
        public string DateCreated { get; set; }


        [JsonProperty("answers")]
        public IEnumerable<AnswerVM> Answers { get; set; }


    }
}
