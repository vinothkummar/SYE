using SYE.EsbWrappers.Authentication;
using System;
using System.IO;
using System.Net;
using SYE.Models.SubmissionSchema;
using AuthenticationServiceReference;
using System.ServiceModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using System.Xml;
using System.Linq;
using System.Xml.Linq;

namespace SYE.EsbWrappers
{
    public interface IEsbClient
    {
        string SendGenericAttachment(string payload, PayloadType type);
    }
    public class EsbClient : IEsbClient
    {
        private IEsbConfiguration<EsbConfig> _esbConfig;
        public EsbClient(IEsbConfiguration<EsbConfig> esbConfig)
        {
            _esbConfig = esbConfig;
        }
        public string SendGenericAttachment(string payload, PayloadType type)
        {
            var token = GetToken();
            string response;

            var username = "gfcPortal2";//_esbConfig.EsbGenericAttachmentUsername;
            var password = "S4K79oufPgvi9F";//_esbConfig.EsbGenericAttachmentPassword;
            var endpoint = "https://api-sys.cqc.org.uk/sys4/olsServiceInterface/services/GenericAttachmentService";//_esbConfig.EsbGenericAttachmentEndpoint;

            if (username == null || password == null || endpoint == null) throw new ArgumentException("Could not read UserName, Password or GenericAttachmentEndpoint AppSettings");

            var path = Directory.GetCurrentDirectory() + "\\Resources\\GenericAttachmentTemplate.xml";
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
                        .Replace("{{payload}}", payload)
                        //.Replace("{{username}}", username)
                        //.Replace("{{password}}", password)
                        .Replace("{{subtype}}", GetFriendlyName(type))
                        .Replace("{{submissionNumber}}", "1000023");
                    client.Headers.Add(_esbConfig.EsbGenericAttachmentSubmitKey, _esbConfig.EsbGenericAttachmentSubmitValue);
                    response = client.UploadString(endpoint, finalPayload);
                }
            }

            return response;
        }
        /// <summary>
        /// Returns a new token using the ESB authentication service.
        /// </summary>
        /// <returns>Returns a string containing the token.</returns>
        private string GetToken()
        {
            var returnString = string.Empty;
            var esbEndpoint = "https://api-sys.cqc.org.uk/sys4/olsServiceInterface/services/authenticationService";//_esbConfig.EsbAuthenticationEndpoint;
            var esbAuthUser = _esbConfig.EsbAuthenticationUsername;
            var esbAuthPassword = _esbConfig.EsbAuthenticationPassword;
            var esbCredUsername = "gfcPortal2@cqc.org.uk";//_esbConfig.EsbAuthenticationCredUsername;
            var esbCredPassword = "S4K79oufPgvi9F";//_esbConfig.EsbAuthenticationCredPassword;

            try
            {
                var path = Directory.GetCurrentDirectory() + "\\Resources\\GetTokenTemplate.xml";
                using (var client = new HttpClient())
                {
                    var nonce = GetNonce();
                    var created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    client.DefaultRequestHeaders.Add("SOAPAction", "\"http://provider.model.service.ols.cqc.org.uk/AuthenticationService:authenticate\"");
                    client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
                    client.DefaultRequestHeaders.Add("User-Agent", "Apache-HttpClient/4.1.1 (java 1.5)");

                    var uri = new Uri(esbEndpoint);
                    var env = "";
                    using (var reader = new StreamReader(path))
                    {
                        var template = @reader.ReadToEnd();
                        var finalPayload = template.Replace("{{username}}", esbCredUsername)
                            .Replace("{{password}}", esbCredPassword)
                            .Replace("{{nonce}}", nonce)
                            .Replace("{{created}}", created);
                        env = finalPayload;
                    }

                    //var env = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Header><wsse:Security xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\"><wsse:UsernameToken wsu:Id=\"UsernameToken-3D78C7F7D45CB562D8156078803058926\"><wsse:Username>gfcPortal1</wsse:Username><wsse:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText\">H&amp;xLzwgFRn8P</wsse:Password><wsse:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\">c2Rmc2Rm</wsse:Nonce><wsu:Created>2019-06-18T10:55:50.589Z</wsu:Created></wsse:UsernameToken></wsse:Security></s:Header><s:Body xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><GetAuthenticationRequest xmlns=\"http://provider.model.service.ols.cqc.org.uk/authenticateSchema\"><userName>gfcPortal2@cqc.org.uk</userName><password>S4K79oufPgvi9F</password></GetAuthenticationRequest></s:Body></s:Envelope>";
                    var content = new StringContent(env);
                    //var content = new StringContent("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\"><s:Header><wsse:Security xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\"><wsse:UsernameToken wsu:Id=\"UsernameToken-3D78C7F7D45CB562D8156078803058926\"><wsse:Username>gfcPortal1</wsse:Username><wsse:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText\">H&amp;xLzwgFRn8P</wsse:Password><wsse:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\">dHJ5dHJ5</wsse:Nonce><wsu:Created>2019-06-18T10:10:50.589Z</wsu:Created></wsse:UsernameToken></wsse:Security></s:Header><s:Body xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><GetAuthenticationRequest xmlns=\"http://provider.model.service.ols.cqc.org.uk/authenticateSchema\"><userName>gfcPortal2@cqc.org.uk</userName><password>S4K79oufPgvi9F</password></GetAuthenticationRequest></s:Body></s:Envelope>");
                    var result = client.PostAsync(uri, content).ConfigureAwait(false).GetAwaiter().GetResult();
                    if (result.IsSuccessStatusCode)
                    {
                        var msg = result.Content.ReadAsStringAsync().Result;
                        var stream = result.Content.ReadAsStreamAsync().Result;
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(msg);
                        //TODO this is clunky
                        returnString = doc.ChildNodes[0].ChildNodes[0].ChildNodes[0].ChildNodes[0].LastChild.Value;
                    }                    
                }
/*               
                var basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                var endpointAddress = new EndpointAddress(new Uri(esbEndpoint));
                var factory = new ChannelFactory<AuthenticationService>(basicHttpBinding, endpointAddress);

                var serviceProxy = factory.CreateChannel();
                ((ICommunicationObject)serviceProxy).Open();
                var opContext = new OperationContext((IClientChannel)serviceProxy);
                //var soapSecurityHeader = new SoapSecurityHeader("UsernameToken-32", esbAuthUser, esbAuthPassword);
                //// Adding the security header
                //opContext.OutgoingMessageHeaders.Add(soapSecurityHeader);
                var prevOpContext = OperationContext.Current; // Optional if there's no way this might already be set
                OperationContext.Current = opContext;

                var request = new authenticateRequest
                {
                    userName = esbCredUsername,
                    password = esbCredPassword
                };
                var token = serviceProxy.authenticateAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();
                returnString = token.tokenId;
                factory.Close();
         */
            }
            catch (Exception e)
            {
                return null;
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
