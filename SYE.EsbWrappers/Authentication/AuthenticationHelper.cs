using AuthenticationServiceReference;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace SYE.EsbWrappers.Authentication
{
    public static class AuthenticationHelper
    {
        /// <summary>
        /// Returns a new token using the ESB authentication service.
        /// </summary>
        /// <returns>Returns a string containing the token.</returns>
        public static string GetToken()
        {
            var returnString = string.Empty;
            //var esbEndpoint = "https://api-sys.cqc.org.uk/sys4/olsServiceInterface/services/authenticationService";//reader.GetValue("ESBAuthenticationEndpoint", typeof(string)).ToString();
            var esbEndpoint = "https://api-sys.cqc.org.uk/sys4/ols-gfc/v1/AuthenticationService";
            var esbAuthUser = "drupalUser";//reader.GetValue("ESBAuthenticationUsername", typeof(string)).ToString();
            var esbAuthPassword = "1687v8WyeN5kyh";//reader.GetValue("ESBAuthenticationPassword", typeof(string)).ToString();
            var esbCredUsername = "admin1@axis12.com";//reader.GetValue("ESBCredentialsUsername", typeof(string)).ToString();
            var esbCredPassword = "London2011";//reader.GetValue("ESBCredentialsPassword", typeof(string)).ToString();

            try
            {
                BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                basicHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                EndpointAddress endpointAddress = new EndpointAddress(new Uri(esbEndpoint));
                var factory = new ChannelFactory<AuthenticationService>(basicHttpBinding, endpointAddress);

                AuthenticationService serviceProxy = factory.CreateChannel();
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
    }
    [ServiceContract]
    public interface IAuthenticatService
    {
        [OperationContract]
        string Authenticate(string msg);
    }

    public class AuthenticatService : IAuthenticatService
    {
        public string Authenticate(string msg)
        {
            //throw new NotImplementedException();
            return "boo";
        }
    }
    internal class CustomCredentials : ClientCredentials
    {
        public CustomCredentials() { }

        protected CustomCredentials(CustomCredentials cc)
            : base(cc) { }

        protected override ClientCredentials CloneCore()
        {
            return new CustomCredentials(this);
        }
    }
}
