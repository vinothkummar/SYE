using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SYE.EsbWrappers.Authentication
{
    //    internal class CustomCredentials : ClientCredentials
    //    {
    //        public CustomCredentials() { }

    //        protected CustomCredentials(CustomCredentials cc)
    //            : base(cc) { }

    //        public override SecurityTokenManager CreateSecurityTokenManager()
    //        {
    //            return new CustomSecurityTokenManager(this);
    //        }

    //        protected override ClientCredentials CloneCore()
    //        {
    //            return new CustomCredentials(this);
    //        }
    //    }

    //    internal class CustomSecurityTokenManager : ClientCredentialsSecurityTokenManager
    //    {
    //        public CustomSecurityTokenManager(CustomCredentials cred)
    //            : base(cred)
    //        { }

    //        public override SecurityTokenSerializer CreateSecurityTokenSerializer(SecurityTokenVersion version)
    //        {
    //            return new CustomTokenSerializer(SecurityVersion.WSSecurity11);
    //        }
    //    }

    //internal class CustomTokenSerializer// : WSSecurityTokenSerializer
    //{
    //    public CustomTokenSerializer(SecurityVersion sv)
    //        : base(sv)
    //    { }

    //    protected override void WriteTokenCore(XmlWriter writer, SecurityToken token)
    //    {
    //        UserNameSecurityToken userToken = token as UserNameSecurityToken;

    //        string tokennamespace = "o";

    //        DateTime created = DateTime.UtcNow;
    //        string createdStr = created.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

    //        string phrase = Guid.NewGuid().ToString();
    //        var nonce = GetSHA1String(phrase);

    //        string password = userToken.Password;

    //        writer.WriteRaw(string.Format(
    //        "<{0}:UsernameToken u:Id=\"" + token.Id +
    //        "\" xmlns:u=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">" +
    //        "<{0}:Username>" + userToken.UserName + "</{0}:Username>" +
    //        "<{0}:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText\">" +
    //        password + "</{0}:Password>" +
    //        "<{0}:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\">" +
    //        nonce + "</{0}:Nonce>" +
    //        "<u:Created>" + createdStr + "</u:Created></{0}:UsernameToken>", tokennamespace));
    //    }

    //    protected string GetSHA1String(string phrase)
    //    {
    //        SHA1CryptoServiceProvider sha1Hasher = new SHA1CryptoServiceProvider();
    //        byte[] hashedDataBytes = sha1Hasher.ComputeHash(Encoding.UTF8.GetBytes(phrase));
    //        return Convert.ToBase64String(hashedDataBytes);
    //    }

    //    public static AuthenticationServiceClient CreateAuthenticationServiceClient(string url, string username, string password)
    //    {
    //        if (string.IsNullOrEmpty(url))
    //            url = "https://notrealurl.com:443/cows/services/RealTimeOnline";

    //        CustomBinding binding = new CustomBinding();

    //        var security = TransportSecurityBindingElement.CreateUserNameOverTransportBindingElement();
    //        security.IncludeTimestamp = false;
    //        security.DefaultAlgorithmSuite = SecurityAlgorithmSuite.Basic256;
    //        security.MessageSecurityVersion = MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;

    //        var encoding = new TextMessageEncodingBindingElement();
    //        encoding.MessageVersion = MessageVersion.Soap11;

    //        var transport = new HttpsTransportBindingElement();
    //        transport.MaxReceivedMessageSize = 20000000;

    //        binding.Elements.Add(security);
    //        binding.Elements.Add(encoding);
    //        binding.Elements.Add(transport);

    //        AuthenticationServiceClient client = new AuthenticationServiceClient(binding,
    //            new EndpointAddress(url));

    //        // to use full client credential with Nonce uncomment this code:
    //        // it looks like this might not be required - the service seems to work without it
    //        client.ChannelFactory.Endpoint.Behaviors.Remove<ClientCredentials>();
    //        client.ChannelFactory.Endpoint.Behaviors.Add(new CustomCredentials());

    //        client.ClientCredentials.UserName.UserName = username;
    //        client.ClientCredentials.UserName.Password = password;

    //        return client;
    //    }
    //}
}
