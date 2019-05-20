using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace SYE.EsbWrappers.Authentication
{
    public class SoapSecurityHeader : MessageHeader
    {
        private readonly string _password, _username;

        public SoapSecurityHeader(string id, string username, string password)
        {
            _password = password;
            _username = username;
        }
        public override bool MustUnderstand => true;

        public override string Name
        {
            get { return "Security"; }
        }

        public override string Namespace
        {
            get { return "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"; }
        }

        protected override void OnWriteStartHeader(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement("wsse", Name, Namespace);
            writer.WriteAttributeString("s", "mustUnderstand", "http://schemas.xmlsoap.org/soap/envelope/", "1");
            writer.WriteXmlnsAttribute("wsse", Namespace);
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement("wsse", "UsernameToken", Namespace);
            writer.WriteAttributeString("wsu", "Id", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd", "UsernameToken-32");
            // Username
            writer.WriteStartElement("wsse", "Username", Namespace);
            writer.WriteValue(_username);
            writer.WriteEndElement();
            // Password
            writer.WriteStartElement("wsse", "Password", Namespace);
            writer.WriteAttributeString("Type", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText");
            writer.WriteValue(_password);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
