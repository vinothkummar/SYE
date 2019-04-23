﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using FluentAssertions;
using SYE.Services;
using Xunit;

namespace SYE.Tests.Services
{
    public class DocumentServiceTests
    {
        private string _dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\Resources\\";
        private string _fileNameNoContact = "submission-schema-no-contact.json";
        private string _location = "Test Location";

        [Fact]
        public void CreateDocumentTest()
        {
            var sut = new DocumentService();
            var json = GetJsonString();

            var result = sut.CreateDocumentFromJsonAsync(json, _dir).Result;
            result.Should().Be(true);
        }

        /// <summary>
        /// this method reads a json file from the folder and returns the next page
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="path"></param>
        /// <param name="locationName"></param>
        /// <returns></returns>
        /// <remarks>
        /// Please refactor this function (and all tests consuming this method) so method accepts whole form schema and returns required page.
        /// If we need to load form from database/cache/session/file-system it has to be done as a seperate function
        /// </remarks>   
        private string GetJsonString()
        {
            var file = string.Empty;
            var path = _dir + _fileNameNoContact;
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException(nameof(path));
            }

            using (var r = new StreamReader(path))
            {
                file = r.ReadToEnd();
            }

            return file;
        }

    }
}
