using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SYE.Models.SubmissionSchema;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using ParagraphProperties = DocumentFormat.OpenXml.Wordprocessing.ParagraphProperties;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using RunProperties = DocumentFormat.OpenXml.Wordprocessing.RunProperties;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace SYE.Services
{
    public interface IDocumentService
    {
        string CreateDocumentFromJson(string json, string folderName);
        string CreateDocumentFromJson(SubmissionVM submissionVm, string path);
    }
    public class DocumentService : IDocumentService
    {
        private readonly int FontSizeHeader = 50;
        private readonly int FontSizeNormal = 25;
        private readonly int FontSizeSmall = 15;

        public string CreateDocumentFromJson(string json, string folderName)
        {
           var submissionVm = JsonConvert.DeserializeObject<SubmissionVM>(json);
           var filePath = GenerateDocument(submissionVm, folderName);
           return filePath;
        }

        public string CreateDocumentFromJson(SubmissionVM submissionVm, string folderName)
        {
            var filePath = GenerateDocument(submissionVm, folderName);
            return filePath;
        }

        private string GenerateDocument(SubmissionVM submissionVm, string folderName)
        {
            var title = submissionVm.UserRef;
            try
            {
                var fullPath = folderName + title + ".docx";
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
                        var line = GetText("NCSC GFC concern template", FontSizeHeader);
                        para.AppendChild(line);
                        EmptyLine(body, para);

                        GetDataSection(body, "Channel :", new List<string>{"GFC"}, false);
                        GetDataSection(body, "GFC reference number :", new List<string> {submissionVm.UserRef}, false);
                        GetDataSection(body, "Completed :", new List<string> {DateTime.Parse(submissionVm.DateCreated).ToShortDateString()}, false);
                        GetDataSection(body, "Location ID :", new List<string> {submissionVm.LocationId}, false);
                        GetDataSection(body, "Provider ID :", new List<string> {submissionVm.ProviderId}, false);
                        GetDataSection(body, "Location name/description :", new List<string>{ submissionVm.LocationName }, false);
                        //Are you happy to be contacted
                        var answerTxt = string.Empty;
                        var questionTxt = GetYesNoAnswer(submissionVm, "Yes I'm happy for you to contact me", "No, I do not want to give my name or contact details", "Contact_002", ref answerTxt);
                        GetDataSection(body, "11. " + questionTxt, new List<string> { answerTxt }, true);
                        //contact details
                        GetContactDetails(body, submissionVm);
                        //have you worked for this service
                        answerTxt = string.Empty;
                        questionTxt = GetYesNoAnswer(submissionVm, "Yes, I have worked for this service", "No, I have never worked for them", "Neg_006", ref answerTxt);
                        GetDataSection(body, "8. " + questionTxt, new List<string> { answerTxt }, true);
                        //risk of harm
                        answerTxt = string.Empty;
                        questionTxt = GetYesNoAnswer(submissionVm, "Yes, I think someone's at risk of harm", "No, I don't think anyone's at risk of harm", "Neg_001", ref answerTxt);
                        GetDataSection(body, "4. " + questionTxt, new List<string> { answerTxt }, true);                        
                        //have you told police
                        var answer = submissionVm.Answers.Where(x => x.PageId == "Neg_002").FirstOrDefault();
                        if (answer != null)
                        {
                            GetDataSection(body, "5. " + answer.Question, new List<string> { answer.Answer }, true);
                        }
                        //good or bad
                        answer = submissionVm.Answers.Where(x => x.PageId == "Start_001").FirstOrDefault();
                        if (answer != null)
                        {
                            GetDataSection(body, "2. " + answer.Question, new List<string> { answer.Answer }, true);
                        }
                        //when did it happen
                        answer = submissionVm.Answers.Where(x => x.PageId == "Neg_005").FirstOrDefault();
                        if (answer != null)
                        {
                            GetDataSection(body, "3. " + answer.Question, new List<string> {answer.Answer}, true);
                        }
                        //feedback
                        GetFeedback(body, submissionVm);
                        //how didi you find out
                        answer = submissionVm.Answers.Where(x => x.PageId == "Contact_004").FirstOrDefault();
                        if (answer != null)
                        {
                            GetDataSection(body, "13. " + answer.Question, new List<string> { answer.Answer }, true);
                        }
                        //which charity
                        answer = submissionVm.Answers.Where(x => x.PageId == "Contact_005").FirstOrDefault();
                        if (answer != null)
                        {
                            GetDataSection(body, "14. " + answer.Question, new List<string> { answer.Answer }, true);
                        }
                        //can we share you feedback
                        answer = submissionVm.Answers.Where(x => x.PageId == "Contact_001").FirstOrDefault();
                        if (answer != null)
                        {
                            GetDataSection(body, "10. " + answer.Question, new List<string> { answer.Answer }, true);
                        }

                        mainPart.Document.Save();
                        //write to file                        
                        wordDocument.SaveAs(fullPath);
                        wordDocument.Close();
                    }
                    mem.Close();
                }
                return fullPath;
            }
            catch (Exception e)
            {
                //log error
                return null;
            }
        }
        /// <summary>
        /// Creates the feedback section
        /// </summary>
        /// <param name="body"></param>
        /// <param name="submissionVm"></param>
        private void GetFeedback(Body body, SubmissionVM submissionVm)
        {
            var answer = submissionVm.Answers.Where(x => x.PageId == "Neg_004" && x.QuestionId == "Neg_004_01").FirstOrDefault();
            if (answer != null)
            {
                var feedback1 = string.Empty;
                var feedback2 = string.Empty;
                var feedback3 = string.Empty;
                feedback1 = answer.Answer;
                var answer2 = submissionVm.Answers.Where(x => x.PageId == "Neg_004" && x.QuestionId == "Neg_004_02").FirstOrDefault();
                var answer3 = submissionVm.Answers.Where(x => x.PageId == "Neg_004" && x.QuestionId == "Neg_004_03").FirstOrDefault();
                if (answer2 != null)
                {
                    feedback2 = answer2.Answer;
                }
                if (answer2 != null)
                {
                    feedback3 = answer3.Answer;
                }

                GetDataSection(body, "7. " + answer.Question, new List<string> { feedback1 }, true);
                GetDataSection(body, "Can you be more exact about where you're telling us about? For example, which room? (optional)", new List<string> { feedback2 }, true, true);
                GetDataSection(body, "When exactly did it happen? For example, can you give a date, month or year? (optional)", new List<string> { feedback3 }, true, true);
            }
        }
        /// <summary>
        /// Creates the contact details section
        /// </summary>
        /// <param name="body"></param>
        /// <param name="submissionVm"></param>
        private void GetContactDetails(Body body, SubmissionVM submissionVm)
        {
            var answer = submissionVm.Answers.Where(x => x.PageId == "Contact_003" && x.QuestionId == "Contact_003_01").FirstOrDefault();
            if (answer != null)
            {
                var fullName = answer.Answer;
                var email = string.Empty;
                var telNum = string.Empty;

                //contact details 2
                answer = submissionVm.Answers.Where(x => x.PageId == "Contact_003" && x.QuestionId == "Contact_003_02").FirstOrDefault();
                if (answer != null) { email = answer.Answer; }
                //contact details 3
                answer = submissionVm.Answers.Where(x => x.PageId == "Contact_003" && x.QuestionId == "Contact_003_03").FirstOrDefault();
                if (answer != null) { telNum = answer.Answer; }

                GetDataSection(body, "12. " + answer.Question,
                    new List<string>
                    {
                        "Full name: " + fullName, " 	Email address: " + email,
                        " UK telephone number: " + telNum
                    }, true);
            }
        }
        /// <summary>
        /// Set the Yes/No answer and returns the question for a specific submission record
        /// </summary>
        /// <param name="submissionVm"></param>
        /// <param name="yesAnswer"></param>
        /// <param name="noAnswer"></param>
        /// <param name="pageNum"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        private string GetYesNoAnswer(SubmissionVM submissionVm, string yesAnswer, string noAnswer, string pageNum, ref string answer)
        {
            var question = string.Empty;
            var record = submissionVm.Answers.Where(x => x.PageId == pageNum).FirstOrDefault();
            if (record != null)
            {
                question = record.Question;
                if (record.Answer == "Yes")
                {
                    answer = yesAnswer;
                }
                else
                {
                    answer = noAnswer;
                }                
            }
            return question;
        }
        /// <summary>
        /// Creates a line of text for the document
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <param name="bold"></param>
        /// <returns></returns>
        private Run GetText(string text, int size, bool bold = false)
        {            
            Run HighLightRun = new Run();
            RunProperties runPro = new RunProperties();
            RunFonts runFont = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial" };
            FontSize fontSize = new FontSize() { Val = size.ToString() };

            Text runText = new Text() { Text = text};
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
        /// <summary>
        /// Creates a section consisting of a question with al list of answers
        /// </summary>
        /// <param name="body"></param>
        /// <param name="sideheader"></param>
        /// <param name="data"></param>
        /// <param name="dataOnNewLine"></param>
        /// <param name="indent"></param>
        private void GetDataSection(Body body, string sideheader, List<string> data, bool dataOnNewLine, bool indent = false)
        {            
            var para = body.AppendChild(new Paragraph());
            if (indent)
            {
                Indentation iUl = new Indentation() { LeftChars = 10, Hanging = "360" };  // correct indentation  

                NumberingProperties npUl = new NumberingProperties(
                    new NumberingLevelReference() { Val = 2 },
                    new NumberingId() { Val = 1 }
                );                
                
                ParagraphProperties ppUnordered = new ParagraphProperties(npUl, iUl);
                para.ParagraphProperties = new ParagraphProperties(ppUnordered.OuterXml);
            }
            var line = GetText(sideheader, FontSizeNormal, dataOnNewLine);
            para.AppendChild(line);

            foreach (var text in data.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                if (dataOnNewLine)
                {
                    para = body.AppendChild(new Paragraph());
                }
                line = GetText(text, FontSizeNormal, !dataOnNewLine);
                if (dataOnNewLine)
                {
                    para.Append(new Run(new TabChar()));
                }
                para.AppendChild(line);
            }
           EmptyLine(body, para);
        }
        private void EmptyLine(Body body, Paragraph para)
        {
            para = body.AppendChild(new Paragraph());
            para.AppendChild(GetText(string.Empty, FontSizeSmall));
        }
    }
}
