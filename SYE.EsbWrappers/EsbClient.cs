using System;
using System.Net;
using SYE.Models.SubmissionSchema;
using System.Net.Http;
using System.Text;
using System.Xml;
using Microsoft.Extensions.Logging;

namespace SYE.EsbWrappers
{
    public interface IEsbClient
    {
        string SendGenericAttachment(SubmissionVM submission, PayloadType type);
    }
    public class EsbClient : IEsbClient
    {
        private IEsbConfiguration<EsbConfig> _esbConfig;
        private readonly ILogger _logger;

        public EsbClient(IEsbConfiguration<EsbConfig> esbConfig, ILogger<EsbClient> logger)
        {
            _esbConfig = esbConfig;
            _logger = logger;
        }
        public string SendGenericAttachment(SubmissionVM submission, PayloadType type)
        {            
            string returnString = null;
            var token = GetToken();

            if (!string.IsNullOrWhiteSpace(token))
            {
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

                if (username == null || password == null || endpoint == null) throw new ArgumentException("Could not read UserName, Password or GenericAttachmentEndpoint AppSettings");

                using (var client = new WebClient())
                {
                    client.UseDefaultCredentials = true;
                    client.Credentials = new NetworkCredential(username, password);
                    client.Headers.Add("Accept", "application/json");
                    client.Headers.Add("Accept", "text/plain");
                    client.Headers.Add("Accept-Language", "en-US");
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)");
                    client.Headers["Content-Type"] = "text/plain;charset=UTF-8";

                    var genPayload = new GenericAttachmentPayload
                    {
                        Token = token,
                        Payload = submission.Base64Attachment,
                        OrganisationId = organisationId,
                        Description = description,
                        Filename = filename,
                        SubType = GetFriendlyName(type),
                        SubmissionNumber = submissionNumber
                    };
                    var finalPayload = GenerateXmlEnvelope(XmlType.GenericAttachment, genPayload);
                    client.Headers.Add(_esbConfig.EsbGenericAttachmentSubmitKey, _esbConfig.EsbGenericAttachmentSubmitValue);
                    try
                    {
                        var response = client.UploadString(endpoint, finalPayload);
                        //get enquiryId from the responseXml
                        XmlDocument doc = new XmlDocument();
                        doc.LoadXml(response);
                        XmlElement root = doc.DocumentElement;
                        returnString = root.GetElementsByTagName("enquiryId").Item(0).FirstChild.Value;
                    }
                    catch (Exception ex)
                    {
                        //log error and move on
                        _logger.LogError(ex, "Error posting submission to CRM. Submission number:" + submissionNumber);
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

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(esbAuthAction, esbAuthValue);

                var uri = new Uri(esbEndpoint);
                var env = GenerateXmlEnvelope(XmlType.GetToken);
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
        private string GenerateXmlEnvelope(XmlType xmlType, GenericAttachmentPayload payload = null)
        {
            //load all security settings
            var esbAuthUser = _esbConfig.EsbAuthenticationUsername;
            var esbAuthPassword = _esbConfig.EsbAuthenticationPassword;
            var esbCredUsername = _esbConfig.EsbAuthenticationCredUsername;
            var esbCredPassword = _esbConfig.EsbAuthenticationCredPassword;
            var username = _esbConfig.EsbGenericAttachmentUsername;
            var password = _esbConfig.EsbGenericAttachmentPassword;
            var endpoint = _esbConfig.EsbGenericAttachmentEndpoint;           

            var sb = new StringBuilder();
            switch (xmlType)
            {
                case XmlType.GetToken:
                    {
                        var nonce = GetNonce();
                        var created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                        sb.Append("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">");
                        sb.Append("<s:Header>");
                        sb.Append("<wsse:Security xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">");
                        sb.Append("<wsse:UsernameToken wsu:Id=\"UsernameToken - 3D78C7F7D45CB562D8156078803058926\">");
                        sb.AppendFormat("<wsse:Username >{0}</wsse:Username>", esbAuthUser);
                        sb.AppendFormat("<wsse:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText\">{0}</wsse:Password>", esbAuthPassword);
                        sb.AppendFormat("<wsse:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\">{0}</wsse:Nonce>", nonce);
                        sb.AppendFormat("<wsu:Created>{0}</wsu:Created>", created);
                        sb.Append("</wsse:UsernameToken>");
                        sb.Append("</wsse:Security>");
                        sb.Append("</s:Header>");
                        sb.Append("<s:Body xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
                        sb.Append("<GetAuthenticationRequest xmlns=\"http://provider.model.service.ols.cqc.org.uk/authenticateSchema\">");
                        sb.AppendFormat("<userName>{0}</userName>", esbCredUsername);
                        sb.AppendFormat("<password>{0}</password>", esbCredPassword);
                        sb.Append("</GetAuthenticationRequest>");
                        sb.Append("</s:Body>");
                        sb.Append("</s:Envelope>");
                        break;
                    }
                case XmlType.GenericAttachment:
                {
                    var nonce = GetNonce();
                    var created = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
                    sb.Append("<soapenv:Envelope xmlns:att=\"http://provider.model.service.ols.cqc.org.uk/generic/attachment\" xmlns:mas=\"http://provider.model.service.ols.cqc.org.uk/masterdata\" xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\">");
                    sb.Append("<soapenv:Header>");
                    sb.Append("<wsse:Security xmlns:wsse=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:wsu=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">");
                    sb.Append("<wsse:UsernameToken wsu:Id=\"UsernameToken-3D78C7F7D45CB562D8156092921928438\">");
                    sb.AppendFormat("<wsse:Username>{0}</wsse:Username>", esbAuthUser);
                    sb.AppendFormat("<wsse:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText\">{0}</wsse:Password>", esbAuthPassword);
                    sb.AppendFormat("<wsse:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\">{0}</wsse:Nonce>", nonce);
                    sb.AppendFormat("<wsu:Created>{0}</wsu:Created>", created);
                    sb.Append("</wsse:UsernameToken>");
                    sb.Append("</wsse:Security>");
                    sb.Append("<mas:Credentials>");
                    sb.AppendFormat("<mas:tokenId>{0}</mas:tokenId>", payload.Token);
                    sb.Append("<mas:originatingSystem>SYE</mas:originatingSystem>");
                    sb.Append("<mas:originatingSystemId>GFC</mas:originatingSystemId>");
                    sb.Append("<mas:status>SUCCESS</mas:status>");
                    sb.Append("</mas:Credentials>");
                    sb.Append("</soapenv:Header>");
                    sb.Append("<soapenv:Body>");
                    sb.Append("<att:CreateEnquiry_Input>");
                    sb.Append("<att:CQCEnquiry>");
                    sb.Append("<att:Enquiry>");
                    sb.AppendFormat("<att:OrganisationId>{0}</att:OrganisationId>", payload.OrganisationId);
                    sb.Append("<att:ListOfDataItems>");
                    sb.Append("<att:Data>");
                    sb.Append("<att:ListOfAttachments>");
                    sb.Append("<att:Attachment>");
                    sb.Append("<att:PrimaryContent>");
                    sb.AppendFormat("<att:FileName>{0}</att:FileName>", payload.Filename);
                    sb.AppendFormat("<att:FileContent>{0}</att:FileContent>", payload.Payload);
                    sb.Append("<att:ContentType>Safeguarding</att:ContentType>");
                    sb.Append("</att:PrimaryContent>");                    
                    sb.Append("<att:ListOfAlternateFileRepresentations>");
                    sb.Append("<att:AlternateFileRepresentations>");
                    sb.Append("<att:FileName/>");
                    sb.Append("<att:FileContent/>");
                    sb.Append("</att:AlternateFileRepresentations>");
                    sb.Append("</att:ListOfAlternateFileRepresentations>");
                    sb.Append("</att:Attachment>");
                    sb.Append("</att:ListOfAttachments>");
                    sb.Append("</att:Data>");
                    sb.Append("</att:ListOfDataItems>");                    
                    sb.Append("<att:Category>Monitor and Inspect</att:Category>");
                    sb.Append("<att:Type>Share your experience</att:Type>");
                    sb.AppendFormat("<att:Subtype>{0}</att:Subtype>", payload.SubType);
                    sb.Append("<att:SourceChannel>Share your experience</att:SourceChannel>");
                    sb.AppendFormat("<att:Description><![CDATA[{0}]]></att:Description>", payload.Description);
                    sb.Append("<att:CommMethod/>");
                    sb.Append("<att:ContactPhone/>");
                    sb.Append("<att:ContactLastName/>");
                    sb.Append("<att:ContactFirstName/>");                    
                    sb.Append("<att:ContactEmail>gfcPortal1@cqc.org</att:ContactEmail>");
                    sb.Append("<att:sourceApplication>Drupal</att:sourceApplication>");
                    sb.Append("<att:sourceSystem>Drupal</att:sourceSystem>");
                    sb.Append("<att:initialReceiptDate>2019-04-18</att:initialReceiptDate>");
                    sb.Append("<att:Creator>gfcPortal1</att:Creator>");
                    sb.AppendFormat("<att:olsSubmissionNumber>{0}</att:olsSubmissionNumber>", payload.SubmissionNumber);
                    sb.Append("</att:Enquiry>");
                    sb.Append("</att:CQCEnquiry>");
                    sb.Append("</att:CreateEnquiry_Input>");
                    sb.Append("</soapenv:Body>");
                    sb.Append("</soapenv:Envelope>");
                    break;
                }
            }

            return sb.ToString();
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
    internal enum XmlType
    {
        GetToken,
        GenericAttachment
    }

    internal class GenericAttachmentPayload
    {
        public string Token { get; set; }
        public string Payload { get; set; }
        public string OrganisationId { get; set; }
        public string Description { get; set; }
        public string Filename { get; set; }
        public string SubType { get; set; }
        public string SubmissionNumber { get; set; }
    }
}
