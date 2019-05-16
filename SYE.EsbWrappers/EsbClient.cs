using SYE.EsbWrappers.Authentication;
using System;
using System.IO;
using System.Net;
using SYE.Models.SubmissionSchema;
using AuthenticationServiceReference;
using System.ServiceModel;

namespace SYE.EsbWrappers
{
    public interface IEsbClient
    {
        string SendGenericAttachment(string payload, PayloadType type);
    }
    public class EsbClient : IEsbClient
    {
        private IEsbConfiguration<EsbConfigVM> _esbConfig;
        public EsbClient(IEsbConfiguration<EsbConfigVM> esbConfig)
        {
            _esbConfig = esbConfig;
        }
        public string SendGenericAttachment(string payload, PayloadType type)
        {
            var token = GetToken();
            string response;

            var username = _esbConfig.EsbGenericAttachmentUsername;
            var password = _esbConfig.EsbGenericAttachmentPassword;
            var endpoint = _esbConfig.EsbGenericAttachmentEndpoint;

            if (username == null || password == null || endpoint == null) throw new ArgumentException("Could not read UserName, Password or GenericAttachmentEndpoint AppSettings");

            var path = Directory.GetCurrentDirectory() + "\\Resources\\GenericAttachmentTemplate.xml";
            using (var client = new WebClient())
            {
                using (var reader = new StreamReader(path))
                {
                    var template = @reader.ReadToEnd();
                    var finalPayload = template.Replace("{{token}}", token)
                        .Replace("{{payload}}", payload)
                        .Replace("{{username}}", username)
                        .Replace("{{password}}", password)
                        .Replace("{{subtype}}", GetFriendlyName(type));

                    client.Headers.Add("SOAPAction", "document/http://provider.model.service.ols.cqc.org.uk/olsEnquiry:CreateEnquiry");
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

            var esbEndpoint = _esbConfig.EsbAuthenticationEndpoint;
            var esbAuthUser = _esbConfig.EsbAuthenticationUsername;
            var esbAuthPassword = _esbConfig.EsbAuthenticationPassword;
            var esbCredUsername = _esbConfig.EsbAuthenticationCredUsername;
            var esbCredPassword = _esbConfig.EsbAuthenticationCredPassword;

            try
            {
                var basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                var endpointAddress = new EndpointAddress(new Uri(esbEndpoint));
                var factory = new ChannelFactory<AuthenticationService>(basicHttpBinding, endpointAddress);

                var serviceProxy = factory.CreateChannel();
                ((ICommunicationObject)serviceProxy).Open();
                var opContext = new OperationContext((IClientChannel)serviceProxy);
                var soapSecurityHeader = new SoapSecurityHeader("UsernameToken-32", esbAuthUser, esbAuthPassword);
                // Adding the security header
                opContext.OutgoingMessageHeaders.Add(soapSecurityHeader);
                var prevOpContext = OperationContext.Current; // Optional if there's no way this might already be set
                OperationContext.Current = opContext;

                var token = serviceProxy.authenticateAsync(new authenticateRequest
                {
                    userName = esbCredUsername,
                    password = esbCredPassword,
                    userType = "admin"
                }).ConfigureAwait(false).GetAwaiter().GetResult();
                returnString = token.tokenId;
                factory.Close();
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
                default:
                    throw new ArgumentOutOfRangeException("payloadType", payloadType, null);
            }
        }
    }
    public enum PayloadType
    {
        Submission
    }
}
