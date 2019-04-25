using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using FluentAssertions;
using SYE.Tests.TestHelpers;
using SYE.Services;
using Xunit;

namespace SYE.Tests.Services
{
    public class DocumentServiceTests
    {
        private string _dir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\Resources\\";
        private string _fileNameNoContact = "submission-schema-no-contact.json";
        private string _fileNameContactDetails = "submission-schema-contact-details.json";

        public DocumentServiceTests()
        {
            FileHelper.DeleteFilesWithExtension(_dir, "docx");//remove any residual files
        }

        [Fact]
        public void CreateDocumentNoContactDetailsTest()
        {            
            var sut = new DocumentService();
            var json = GetJsonString(_fileNameNoContact);

            var result = sut.CreateDocumentFromJson(json, _dir);
            result.Should().NotBeNullOrWhiteSpace();
            FileHelper.FileExists(result).Should().BeTrue();
        }
        [Fact]
        public void CreateDocumentWithContactDetailsTest()
        {
            var sut = new DocumentService();
            var json = GetJsonString(_fileNameContactDetails);

            var result = sut.CreateDocumentFromJson(json, _dir);
            result.Should().NotBeNullOrWhiteSpace();
            FileHelper.FileExists(result).Should().BeTrue();
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
        private string GetJsonString(string fileName)
        {
            var file = string.Empty;
            var path = _dir + fileName;
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

        //public void Dispose()
        //{
        //    FileHelper.DeleteFilesWithExtension(_dir, "docx");
        //}

    }
}
