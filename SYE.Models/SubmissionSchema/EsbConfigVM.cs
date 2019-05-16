﻿using Newtonsoft.Json;

namespace SYE.Models.SubmissionSchema
{
    public class EsbConfigVM
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

        //generic attachment        
        [JsonProperty("esbGenericAttachmentEndpoint")]
        public string EsbGenericAttachmentEndpoint { get; set; }
        
        [JsonProperty("esbGenericAttachmentUsername")]
        public string EsbGenericAttachmentUsername { get; set; }
        
        [JsonProperty("esbGenericAttachmentPassword")]
        public string EsbGenericAttachmentPassword { get; set; }
    }
}
