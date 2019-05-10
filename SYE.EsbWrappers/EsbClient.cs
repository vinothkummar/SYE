using Microsoft.AspNetCore.Http;
using SYE.EsbWrappers.Authentication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace SYE.EsbWrappers
{
    public static class EsbClient
    {
        public static string SendGenericAttachment(string payload, PayloadType type)
        {
            //WSSecurityTokenSerializer ser = new WSSecurityTokenSerializer();

            var token = AuthenticationHelper.GetToken();
            string response;
            var username = "";//ConfigurationManager.AppSettings["ESBAuthenticationUsername"];
            var password = "";//ConfigurationManager.AppSettings["ESBAuthenticationPassword"];
            var endpoint = "";//ConfigurationManager.AppSettings["GenericAttachmentEndpoint"];
            if (username == null || password == null || endpoint == null) throw new ArgumentException("Could not read UserName, Password or GenericAttachmentEndpoint AppSettings");

            var path = Directory.GetCurrentDirectory() + "\\Resources\\GenericAttachmentTemplate.xml";
            using (var client = new WebClient())
            {
                using (var reader = new StreamReader(path))
                {
                    var template = reader.ReadToEnd();
                    var finalPayload = template.Replace("{{token}}", token)
                        .Replace("{{payload}}", payload)
                        .Replace("{{username}}", username)
                        .Replace("{{password}}", password)
                        .Replace("{{subtype}}", type.FriendlyName());

                    client.Headers.Add("SOAPAction", "document/http://provider.model.service.ols.cqc.org.uk/olsEnquiry:CreateEnquiry");
                    response = client.UploadString(endpoint, finalPayload);
                }
            }

            return response;
        }

        private static string FriendlyName(this PayloadType payloadType)
        {
            switch (payloadType)
            {
                case PayloadType.Submission:
                    return "SYE Submission";
                default:
                    throw new ArgumentOutOfRangeException("payloadType", payloadType, null);
            }
        }

        public enum PayloadType
        {
            Submission
        }
    }
}
