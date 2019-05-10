using System;
using System.Collections.Generic;
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
            //var reader = new AppSettingsReader();
            var esbEndpoint = "";//reader.GetValue("ESBAuthenticationEndpoint", typeof(string)).ToString();
            var esbAuthUser = "";//reader.GetValue("ESBAuthenticationUsername", typeof(string)).ToString();
            var esbAuthPassword = "";//reader.GetValue("ESBAuthenticationPassword", typeof(string)).ToString();
            var esbCredUsername = "";//reader.GetValue("ESBCredentialsUsername", typeof(string)).ToString();
            var esbCredPassword = "";//reader.GetValue("ESBCredentialsPassword", typeof(string)).ToString();

           // var service = CustomTokenSerializer.CreateAuthenticationServiceClient(esbEndpoint, esbAuthUser, esbAuthPassword);

            //var credentials = new Credentials
            //{
            //    originatingSystem = "DPL",
            //    originatingSystemId = "OLS",
            //    status = "SUCCESS"
            //};

            //// Unused outputs
            //string one, two;
            //bool three, four;

            //var token = service.authenticate(credentials, esbCredUsername, esbCredPassword, "admin", out three, out four, out one, out two);
            //return token;
            return "gobbledygook";
        }
    }
}
