using System.Collections.Generic;
using Newtonsoft.Json;

namespace GDSHelpers.Models.FormSchema
{
    public class PageVM
    {
        [JsonProperty("page_id")]
        public string PageId { get; set; }


        [JsonProperty("page_name")]
        public string PageName { get; set; }


        [JsonProperty("pre_amble")]
        public string PreAmble { get; set; }


        [JsonProperty("questions")]
        public IEnumerable<QuestionVM> Questions { get; set; }


        [JsonProperty("buttons")]
        public IEnumerable<ButtonVM> Buttons { get; set; }


        [JsonProperty("next_page_id")]
        public string NextPageId { get; set; }


    }
}
