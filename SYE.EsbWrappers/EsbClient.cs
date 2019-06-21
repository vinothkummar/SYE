using System;
using System.IO;
using System.Net;
using SYE.Models.SubmissionSchema;
using System.Net.Http;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Hosting;

namespace SYE.EsbWrappers
{
    public interface IEsbClient
    {
        string SendGenericAttachment(SubmissionVM submission, PayloadType type);
    }
    public class EsbClient : IEsbClient
    {
        private IEsbConfiguration<EsbConfig> _esbConfig;
        private readonly IHostingEnvironment _hostingEnvironment;
        public EsbClient(IEsbConfiguration<EsbConfig> esbConfig, IHostingEnvironment hostingEnvironment)
        {
            _esbConfig = esbConfig;
            _hostingEnvironment = hostingEnvironment;
        }
        public string SendGenericAttachment(SubmissionVM submission, PayloadType type)
        {
            string returnString = null;
            var token = GetToken();

            if (!string.IsNullOrWhiteSpace(token))
            {
                var payload = submission.Base64Attachment;
                var providerId = submission.ProviderId;
                var locationName = submission.LocationName;
                var description = string.Empty;                
                var organisationId = string.Empty;
                if (submission.LocationId == "0")
                {
                    organisationId = string.Empty;//no location selected
                    description = "(GFC)";
                }
                else
                {
                    organisationId = submission.LocationId;
                    description = "(GFC) Location ID: " + submission.LocationId + " Provider ID: " + submission.ProviderId + " Location name: " + submission.LocationName;
                }

                //var submissionNumber = Guid.NewGuid().ToString().Substring(0, 8);//use this for testing because esb rejects duplicate submissionIds
                var submissionNumber = "GFC-" + submission.SubmissionId;
                var filename = submissionNumber + ".docx";                
                var username = _esbConfig.EsbGenericAttachmentUsername;
                var password = _esbConfig.EsbGenericAttachmentPassword;
                var endpoint = _esbConfig.EsbGenericAttachmentEndpoint;
                var esbAuthUser = _esbConfig.EsbAuthenticationUsername;
                var esbAuthPassword = _esbConfig.EsbAuthenticationPassword;

                if (username == null || password == null || endpoint == null) throw new ArgumentException("Could not read UserName, Password or GenericAttachmentEndpoint AppSettings");

                var path = _hostingEnvironment.WebRootPath + "\\Resources\\GenericAttachmentTemplate.xml";
                var nonce = GetNonce();
                var created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                using (var client = new WebClient())
                {
                    client.UseDefaultCredentials = true;
                    client.Credentials = new NetworkCredential(username, password);
                    client.Headers.Add("Accept", "application/json");
                    client.Headers.Add("Accept", "text/plain");
                    client.Headers.Add("Accept-Language", "en-US");
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
                    client.Headers["Content-Type"] = "text/plain;charset=UTF-8";

                    using (var reader = new StreamReader(path))
                    {                        
                        var template = @reader.ReadToEnd();
                        var finalPayload = template.Replace("{{token}}", token)
                            .Replace("{{authUsername}}", esbAuthUser)
                            .Replace("{{authPassword}}", esbAuthPassword)
                            .Replace("{{payload}}", payload)
                            .Replace("{{nonce}}", nonce)
                            .Replace("{{created}}", created)
                            .Replace("{{organisationId}}", organisationId)
                            .Replace("{{description}}", description)
                            .Replace("{{filename}}", filename)
                            .Replace("{{subtype}}", GetFriendlyName(type))
                            .Replace("{{submissionNumber}}", submissionNumber);
                        client.Headers.Add(_esbConfig.EsbGenericAttachmentSubmitKey, _esbConfig.EsbGenericAttachmentSubmitValue);
                        var response = client.UploadString(endpoint, finalPayload);
                        //get enquiryId from the responseXml
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(response);
                        XmlElement root = doc.DocumentElement;
                        returnString = root.GetElementsByTagName("enquiryId").Item(0).FirstChild.Value;
                    }
                }
            }

            return returnString;
        }
        /// <summary>
        /// Returns a new token using the ESB authentication service.
        /// </summary>
        /// <returns>Returns a string containing the token.</returns>
        private string GetToken()
        {
            string returnString = null;
            var esbAuthAction = _esbConfig.EsbAuthenticationSubmitKey;
            var esbAuthValue = _esbConfig.EsbAuthenticationSubmitValue;
            var esbEndpoint = _esbConfig.EsbAuthenticationEndpoint;
            var esbAuthUser = _esbConfig.EsbAuthenticationUsername;
            var esbAuthPassword = _esbConfig.EsbAuthenticationPassword;
            var esbCredUsername = _esbConfig.EsbAuthenticationCredUsername;
            var esbCredPassword = _esbConfig.EsbAuthenticationCredPassword;

            var path = _hostingEnvironment.WebRootPath + "\\Resources\\GetTokenTemplate.xml";
            using (var client = new HttpClient())
            {
                var nonce = GetNonce();
                var created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                client.DefaultRequestHeaders.Add(esbAuthAction, esbAuthValue);

                var uri = new Uri(esbEndpoint);
                var env = "";
                using (var reader = new StreamReader(path))
                {
                    var template = @reader.ReadToEnd();
                    env = template.Replace("{{username}}", esbCredUsername)
                        .Replace("{{password}}", esbCredPassword)
                        .Replace("{{authUsername}}", esbAuthUser)
                        .Replace("{{authPassword}}", esbAuthPassword)
                        .Replace("{{nonce}}", nonce)
                        .Replace("{{created}}", created);
                }

                var content = new StringContent(env);
                var result = client.PostAsync(uri, content).ConfigureAwait(false).GetAwaiter().GetResult();
                if (result.IsSuccessStatusCode)
                {
                    //get tokenId from the responseXml
                    var response = result.Content.ReadAsStringAsync().Result;                    
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(response);
                    XmlElement root = doc.DocumentElement;
                    returnString = root.GetElementsByTagName("tokenId").Item(0).FirstChild.Value;
                }
            }

            return returnString;
        }

        private string GetFriendlyName(PayloadType payloadType)
        {
            switch (payloadType)
            {
                case PayloadType.Submission:
                    return "SYE Submission";
                case PayloadType.Classified:
                    return "To be classified";
                default:
                    throw new ArgumentOutOfRangeException("payloadType", payloadType, null);
            }
        }
        private string GetNonce()
        {
            var source = RandomString(8);
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(source));
        }

        //This function will return a random string from the given numeric characters
        public string RandomString(int size)
        {
            string legalCharacters = Guid.NewGuid().ToString();
            Random random = new Random();
            StringBuilder builder = new StringBuilder();
            char ch = '\0';

            for (int i = 0; i <= size - 1; i++)
            {
                ch = legalCharacters[random.Next(0, legalCharacters.Length)];
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }
    public enum PayloadType
    {
        Submission,
        Classified
    }
}
