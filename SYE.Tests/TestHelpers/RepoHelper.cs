﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace SYE.Tests.TestHelpers
{
    public static class RepoHelper
    {
        public static ResourceResponse<T> ToResourceResponseTest<T>(this T resource, HttpStatusCode statusCode) where T : Resource, new()
        {
            return null;
        }
        public static ResourceResponse<T> ToResourceResponse<T>(this T resource, HttpStatusCode statusCode, IDictionary<string, string> responseHeaders = null) where T : Resource, new()
        {
            var resourceResponse = new ResourceResponse<T>(resource);
            var documentServiceResponseType = Type.GetType("Microsoft.Azure.Documents.DocumentServiceResponse, Microsoft.Azure.DocumentDB.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

            var flags = BindingFlags.NonPublic | BindingFlags.Instance;

            var headers = new NameValueCollection { { "x-ms-request-charge", "0" } };

            if (responseHeaders != null)
            {
                foreach (var responseHeader in responseHeaders)
                {
                    headers[responseHeader.Key] = responseHeader.Value;
                }
            }

            var headersDictionaryType = Type.GetType("Microsoft.Azure.Documents.Collections.DictionaryNameValueCollection, Microsoft.Azure.DocumentDB.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");

            var headersDictionaryInstance = Activator.CreateInstance(headersDictionaryType, headers);

            var arguments = new[] { Stream.Null, headersDictionaryInstance, statusCode, null };

            var documentServiceResponse = documentServiceResponseType.GetTypeInfo().GetConstructors(flags)[0].Invoke(arguments);

            var responseField = typeof(ResourceResponse<T>).GetTypeInfo().GetField("response", flags);

            responseField?.SetValue(resourceResponse, documentServiceResponse);

            return resourceResponse;
        }
    }
}
