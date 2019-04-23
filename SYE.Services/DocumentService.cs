using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SYE.Models.SubmissionSchema;

namespace SYE.Services
{
    public interface IDocumentService
    {
        Task<bool> CreateDocumentFromJsonAsync(string json, string path);
    }
    public class DocumentService : IDocumentService
    {
        public async Task<bool> CreateDocumentFromJsonAsync(string json, string path)
        {
           var submissionVm = JsonConvert.DeserializeObject<SubmissionVM>(json);
            var title = submissionVm.UserRef;
            try
            {
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
                        Body body = mainPart.Document.AppendChild(new Body());
                        Paragraph para = body.AppendChild(new Paragraph());

                        //insert the new created run part
                        var line = GetText("NCSC GFC concern template", 64);
                        para.AppendChild(line);

                        para = body.AppendChild(new Paragraph());
                        line = GetText("Channel: ", 32);
                        para.AppendChild(line);
                        line = GetText(" GFC", 32, true);
                        para.AppendChild(line);

                        para = body.AppendChild(new Paragraph());
                        line = GetText("GFC reference number: ", 32);
                        para.AppendChild(line);
                        line = GetText(" " + submissionVm.UserRef, 32, true);
                        para.AppendChild(line);

                        para = body.AppendChild(new Paragraph());
                        line = GetText("Completed: ", 32);
                        para.AppendChild(line);
                        line = GetText(" " + DateTime.Parse(submissionVm.DateCreated).ToShortDateString() , 32, true);
                        para.AppendChild(line);

                        para = body.AppendChild(new Paragraph());
                        line = GetText("Location ID: ", 32);
                        para.AppendChild(line);
                        line = GetText(" " + submissionVm.LocationId, 32, true);
                        para.AppendChild(line);

                        para = body.AppendChild(new Paragraph());
                        line = GetText("Provider ID: ", 32);
                        para.AppendChild(line);
                        line = GetText(" " + submissionVm.ProviderId, 32, true);
                        para.AppendChild(line);

                        para = body.AppendChild(new Paragraph());
                        line = GetText("Location name/description: ", 32);
                        para.AppendChild(line);
                        line = GetText(" " + submissionVm.LocationName, 32, true);
                        para.AppendChild(line);
                        var contact = false;
                        var answer = submissionVm.Answers.Where(x => x.PageId == "Contact_002").First();
                        if (answer != null)
                        {
                            para = body.AppendChild(new Paragraph());
                            line = GetText("11. " + answer.Question, 32, true);
                            para.AppendChild(line);

                            var txt = "";
                            if (answer.Answer == "Yes")
                            {
                                contact = true;
                                txt = "Yes I'm happy for you to contact me";
                            }                                
                            else
                            {
                                txt = "No, I do not want to give my name or contact details";
                            }
                            para = body.AppendChild(new Paragraph());
                            line = GetText(txt, 32);
                            para.AppendChild(line);
                        }
                        if (contact)
                        {
                            answer = submissionVm.Answers.Where(x => x.PageId == "Contact_003").First();
                            if(answer != null)
                            para = body.AppendChild(new Paragraph());
                            line = GetText("12 " + answer.Question, 32, true);
                            para.AppendChild(line);
                            line = GetText(" Full name: " + answer.Answer, 32);
                            para.AppendChild(line);
                        }
                        answer = submissionVm.Answers.Where(x => x.PageId == "Neg_006").First();
                        if (answer != null)
                        {
                            para = body.AppendChild(new Paragraph());
                            line = GetText("8. " + answer.Question, 32, true);
                            para.AppendChild(line);
                            var txt = "";
                            if (answer.Answer == "Yes")
                            {
                                txt = "Yes, I have worked for this service";
                            }
                            else
                            {
                                txt = "No, I have never worked for them";
                            }
                            para = body.AppendChild(new Paragraph());
                            line = GetText(txt, 32);
                            para.AppendChild(line);

                        }

                        var tellPolice = false;
                        answer = submissionVm.Answers.Where(x => x.PageId == "Neg_001").First();
                        if (answer != null)
                        {
                            para = body.AppendChild(new Paragraph());
                            line = GetText("4. " + answer.Question, 32, true);
                            para.AppendChild(line);
                            var txt = "";
                            if (answer.Answer == "Yes")
                            {
                                tellPolice = true;
                                txt = "Yes, I think someone's at risk of harm";
                            }
                            else
                            {
                                txt = "No, I don't think anyone's at risk of harm";
                            }
                            para = body.AppendChild(new Paragraph());
                            line = GetText(txt, 32);
                            para.AppendChild(line);
                        }

                        if (tellPolice)
                        {
                            answer = submissionVm.Answers.Where(x => x.PageId == "Neg_002").First();
                            if (answer != null)
                            {
                                para = body.AppendChild(new Paragraph());
                                line = GetText("5. " + answer.Question, 32, true);
                                para.AppendChild(line);
                                para = body.AppendChild(new Paragraph());
                                line = GetText(answer.Answer, 32);
                                para.AppendChild(line);
                            }
                        }
                        //when did it happen
                        answer = submissionVm.Answers.Where(x => x.PageId == "Neg_005").First();
                        if (answer != null)
                        {
                            para = body.AppendChild(new Paragraph());
                            line = GetText("3. " + answer.Question, 32, true);
                            para.AppendChild(line);
                            para = body.AppendChild(new Paragraph());
                            line = GetText(answer.Answer, 32);
                            para.AppendChild(line);
                        }
                        //feedback
                        answer = submissionVm.Answers.Where(x => x.PageId == "Neg_004" && x.QuestionId == "Neg_004_01").First();
                        if (answer != null)
                        {
                            var answer2 = submissionVm.Answers.Where(x => x.PageId == "Neg_004" && x.QuestionId == "Neg_004_02").FirstOrDefault();
                            var answer3 = submissionVm.Answers.Where(x => x.PageId == "Neg_004" && x.QuestionId == "Neg_004_03").FirstOrDefault();

                            para = body.AppendChild(new Paragraph());
                            line = GetText("7. " + answer.Question, 32, true);
                            para.AppendChild(line);
                            para = body.AppendChild(new Paragraph());
                            line = GetText(answer.Answer, 32);
                            para.AppendChild(line);
                            if (answer2 != null)
                            {
                                para = body.AppendChild(new Paragraph());
                                line = GetText(answer2.Answer, 32);
                                para.AppendChild(line);
                            }
                            if (answer2 != null)
                            {
                                para = body.AppendChild(new Paragraph());
                                line = GetText(answer3.Answer, 32);
                                para.AppendChild(line);
                            }
                        }


                        mainPart.Document.Save();
                        //write to file
                        var fullPath = path + title + ".docx";
                        wordDocument.SaveAs(fullPath);                        
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private Run GetText(string text, int size, bool bold = false)
        {
            Run HighLightRun = new Run();
            RunProperties runPro = new RunProperties();
            RunFonts runFont = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" };
            FontSize fontSize = new FontSize() { Val = size.ToString() };
            Text runText = new Text() { Text = text };
            runPro.Append(runFont);
            if (bold)
            {
                runPro.Append(new Bold());
            }            
            runPro.Append(fontSize);

            HighLightRun.Append(runPro);
            HighLightRun.Append(runText);
            return HighLightRun;
        }
    }
}
