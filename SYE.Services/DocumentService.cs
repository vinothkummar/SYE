using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;

namespace SYE.Services
{
    public interface IDocumentService
    {
        Task<bool> CreateDocumentFromJsonAsync(string json);
    }
    public class DocumentService : IDocumentService
    {
        public async Task<bool> CreateDocumentFromJsonAsync(string json)
        {
            var title = "123";
            //using (MemoryStream mem = new MemoryStream())
            //{
            //    // Create Document
            //    using (WordprocessingDocument wordDocument =
            //        WordprocessingDocument.Create(mem, WordprocessingDocumentType.Document, true))
            //    {
            //        // Add a main document part. 
            //        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

            //        // Create the document structure and add some text.
            //        mainPart.Document = new Document();
            //        Body body = mainPart.Document.AppendChild(new Body());
            //        Paragraph para = body.AppendChild(new Paragraph());
            //        Run run = para.AppendChild(new Run());
            //        run.AppendChild(new Text("Hello world!"));
            //        mainPart.Document.Save();
            //        // Stream it down to the browser
            //        context.Response.AppendHeader("Content-Disposition", "attachment;filename=HelloWorld.docx");
            //        context.Response.ContentType = "application/vnd.ms-word.document";
            //        CopyStream(mem, context.Response.OutputStream);
            //        context.Response.End();
            //    }
            //}
            // Create Stream
            using (MemoryStream mem = new MemoryStream())
            {
                // Create Document
                using (WordprocessingDocument wordDocument =
                    WordprocessingDocument.Create(mem, WordprocessingDocumentType.Document, true))
                {
                    // Add a main document part. 
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                    // Create the document structure and add some text.
                    mainPart.Document = new Document();
                    Body docBody = new Body();

                    // Add your docx content here
                }
                //HttpContext context = new DefaultHttpContext();
                // Download File                
                //context.Response.AppendHeader("Content-Disposition", String.Format("attachment;filename=\"0}.docx\"", title));
                //mem.Position = 0;
                //mem.CopyTo(Context.Response.OutputStream);
                //Context.Response.Flush();
                //Context.Response.End();
            }


            return true;
    }
}
