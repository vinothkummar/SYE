using Newtonsoft.Json;

namespace SYE.Models.SubmissionSchema
{
    public class EsbConfig
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        //authentication token
        [JsonProperty("esbAuthenticationEndpoint")]
        public string EsbAuthenticationEndpoint { get; set; }
        
        [JsonProperty("esbAuthenticationUsername")]
        public string EsbAuthenticationUsername { get; set; }
        
        [JsonProperty("esbAuthenticationPassword")]
        public string EsbAuthenticationPassword { get; set; }
        
        [JsonProperty("esbAuthenticationCredUsername")]
        public string EsbAuthenticationCredUsername { get; set; }
        
        [JsonProperty("esbAuthenticationCredPassword")]
        public string EsbAuthenticationCredPassword { get; set; }

        [JsonProperty("esbAuthenticationSubmitKey")]
        public string EsbAuthenticationSubmitKey { get; set; }

        [JsonProperty("esbAuthenticationSubmitValue")]
        public string EsbAuthenticationSubmitValue { get; set; }

        //generic attachment        
        [JsonProperty("esbGenericAttachmentEndpoint")]
        public string EsbGenericAttachmentEndpoint { get; set; }
        
        [JsonProperty("esbGenericAttachmentUsername")]
        public string EsbGenericAttachmentUsername { get; set; }
        
        [JsonProperty("esbGenericAttachmentPassword")]
        public string EsbGenericAttachmentPassword { get; set; }

        [JsonProperty("esbGenericAttachmentSubmitKey")]
        public string EsbGenericAttachmentSubmitKey { get; set; }

        [JsonProperty("esbGenericAttachmentSubmitValue")]
        public string EsbGenericAttachmentSubmitValue { get; set; }
    }
}
