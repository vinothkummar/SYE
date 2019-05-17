using System;
using System.Collections.Generic;
using System.Text;

namespace SYE.EsbWrappers
{
    public interface IEsbConfiguration<T> where T : class
    {
        string EsbAuthenticationEndpoint { get; set; }
        string EsbAuthenticationUsername { get; set; }
        string EsbAuthenticationPassword { get; set; }
        string EsbAuthenticationCredUsername { get; set; }
        string EsbAuthenticationCredPassword { get; set; }
        //generic attachment        
        string EsbGenericAttachmentEndpoint { get; set; }
        string EsbGenericAttachmentUsername { get; set; }
        string EsbGenericAttachmentPassword { get; set; }
        string EsbGenericAttachmentSubmitKey { get; set; }
        string EsbGenericAttachmentSubmitValue { get; set; }
    }
    public class EsbConfiguration<T> : IEsbConfiguration<T> where T : class
    {
        public string EsbAuthenticationEndpoint { get; set; }
        public string EsbAuthenticationUsername { get; set; }
        public string EsbAuthenticationPassword { get; set; }
        public string EsbAuthenticationCredUsername { get; set; }
        public string EsbAuthenticationCredPassword { get; set; }
        //generic attachment        
        public string EsbGenericAttachmentEndpoint { get; set; }
        public string EsbGenericAttachmentUsername { get; set; }
        public string EsbGenericAttachmentPassword { get; set; }
        public string EsbGenericAttachmentSubmitKey { get; set; }
        public string EsbGenericAttachmentSubmitValue { get; set; }
    }

}
