﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
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
        string CreateSubmissionDocument(string json);
        string CreateSubmissionDocument(SubmissionVM submissionVm);
    }
    public class DocumentService : IDocumentService
    {
        private readonly int FontSizeHeader = 50;
        private readonly int FontSizeNormal = 25;
        private readonly int FontSizeSmall = 15;

        public string CreateSubmissionDocument(string json)
        {
            var submissionVm = JsonConvert.DeserializeObject<SubmissionVM>(json);
            return GenerateDocument(submissionVm);
        }

        public string CreateSubmissionDocument(SubmissionVM submissionVm)
        {
            return GenerateDocument(submissionVm);
        }

        private string GenerateDocument(SubmissionVM submissionVm)
        {
            string convertedDoc = null;
            using (MemoryStream documentStream = new MemoryStream())
            {
                // Create Document
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(documentStream, WordprocessingDocumentType.Document, true))
                {
                    // Add a main document part. 
                    MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                    // Create the document structure and add some text.
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());
                    Paragraph para = body.AppendChild(new Paragraph());

                    ////insert the new created run part
                    //var line = GetText("NCSC GFC concern template", FontSizeHeader);
                    //para.AppendChild(line);
                    //EmptyLine(body, para);

                    GetDataSection(body, "Response Id :", new List<string> { submissionVm.Id }, false);
                    GetDataSection(body, "Channel :", new List<string> { "GFC" }, false);
                    GetDataSection(body, "GFC reference number :", new List<string> { submissionVm.SubmissionId }, false);
                    GetDataSection(body, "Completed :", new List<string> { DateTime.Parse(submissionVm.DateCreated).ToShortDateString()  + ": "+ DateTime.Parse(submissionVm.DateCreated).ToShortTimeString() }, false);
                    var locationId = string.Empty;
                    var providerId = string.Empty;
                    if (string.IsNullOrWhiteSpace(submissionVm.LocationId) || submissionVm.LocationId == "0")
                    {
                        locationId = "none";
                    }
                    else
                    {
                        locationId = submissionVm.LocationId;
                    }
                    if (string.IsNullOrWhiteSpace(submissionVm.ProviderId) || submissionVm.ProviderId == "0")
                    {
                        providerId = "none";
                    }
                    else
                    {
                        providerId = submissionVm.ProviderId;
                    }
                    GetDataSection(body, "Location ID :", new List<string> { locationId }, false);
                    GetDataSection(body, "Provider ID :", new List<string> { providerId }, false);
                    //location
                    GetLocation(body, submissionVm);

                    //Are you happy to be contacted
                    var answerTxt = string.Empty;
                    var questionTxt = GetYesNoAnswer(submissionVm, "Yes I'm happy for you to contact me", "No, I do not want to give my name or contact details", "can_we_contact_you", ref answerTxt);
                    GetDataSection(body, questionTxt, new List<string> { answerTxt }, true);
                    //contact details
                    GetContactDetails(body, submissionVm);
                    //have you worked for this service
                    answerTxt = string.Empty;
                    questionTxt = GetYesNoAnswer(submissionVm, "Yes, I have worked for this service", "No, I have never worked for them", "have_you_worked_for_the_service", ref answerTxt);
                    GetDataSection(body, questionTxt, new List<string> { answerTxt }, true);
                    //risk of harm
                    answerTxt = string.Empty;
                    questionTxt = GetYesNoAnswer(submissionVm, "Yes, I think someone's at risk of harm", "No, I don't think anyone's at risk of harm", "is_someone_at_risk", ref answerTxt);
                    GetDataSection(body, questionTxt, new List<string> { answerTxt }, true);
                    //have you told police
                    var answer = submissionVm.Answers.FirstOrDefault(x => x.PageId == "have_you_told_the_police");
                    if (answer != null)
                    {
                        GetDataSection(body, answer.Question, new List<string> { answer.Answer }, true);
                    }
                    //good or bad
                    answer = submissionVm.Answers.FirstOrDefault(x => x.PageId == "what_do_you_want_to_tell_us_about");
                    if (answer != null)
                    {
                        GetDataSection(body, answer.Question, new List<string> { answer.Answer }, true);
                    }
                    //when did it happen
                    answer = submissionVm.Answers.FirstOrDefault(x => x.PageId == "when_did_it_happen");
                    if (answer != null)
                    {
                        GetDataSection(body, answer.Question, new List<string> { answer.Answer }, true);
                    }
                    //feedback
                    GetFeedback(body, submissionVm);
                    //how did you find out
                    answer = submissionVm.Answers.FirstOrDefault(x => x.PageId == "how_did_you_hear_about_this_form");
                    if (answer != null)
                    {
                        GetDataSection(body, answer.Question, new List<string> { answer.Answer }, true);
                    }
                    //which charity
                    answer = submissionVm.Answers.FirstOrDefault(x => x.PageId == "which_charity_told_you");
                    if (answer != null)
                    {
                        GetDataSection(body, answer.Question, new List<string> { answer.Answer }, true);
                    }
                    //can we share you feedback
                    answer = submissionVm.Answers.FirstOrDefault(x => x.PageId == "can_we_share_your_feedback");
                    if (answer != null)
                    {
                        GetDataSection(body, answer.Question, new List<string> { answer.Answer }, true);
                    }

                    mainPart.Document.Save();

                    wordDocument.Close();
                }
                //convert to bas64
                var documentBytes = documentStream.ToArray();
                convertedDoc = Convert.ToBase64String(documentBytes);
                documentStream.Close();
            }
            return convertedDoc;
        }

        /// <summary>
        /// Creates the feedback section
        /// </summary>
        /// <param name="body"></param>
        /// <param name="submissionVm"></param>
        private void GetLocation(Body body, SubmissionVM submissionVm)
        {
            var answer = submissionVm.Answers.FirstOrDefault(x => x.PageId == "service_not_found");
            if (answer == null)
            {
                //location has been selected
                GetDataSection(body, "Location name/description : ", new List<string> { submissionVm.LocationName }, false);
            }
            else
            {
                //location has not been found
                GetDataSection(body, "Location name/description : ", new List<string> { "Not Found" }, false);
                GetDataSection(body, answer.Question, new List<string> { answer.Answer }, true);
            }
        }

        /// <summary>
        /// Creates the feedback section
        /// </summary>
        /// <param name="body"></param>
        /// <param name="submissionVm"></param>
        private void GetFeedback(Body body, SubmissionVM submissionVm)
        {
            var answer = submissionVm.Answers.FirstOrDefault(x => x.PageId == "give_us_your_feedback" && x.QuestionId == "give_us_your_feedback_01");
            if (answer != null)
            {
                var feedback1 = string.Empty;
                var feedback2 = string.Empty;
                var feedback3 = string.Empty;
                feedback1 = answer.Answer;
                var answer2 = submissionVm.Answers.FirstOrDefault(x => x.PageId == "give_us_your_feedback" && x.QuestionId == "give_us_your_feedback_02");
                var answer3 = submissionVm.Answers.FirstOrDefault(x => x.PageId == "give_us_your_feedback" && x.QuestionId == "give_us_your_feedback_03");
                if (answer2 != null)
                {
                    feedback2 = answer2.Answer;
                }
                if (answer2 != null)
                {
                    feedback3 = answer3.Answer;
                }

                GetDataSection(body, answer.Question, new List<string> { feedback1 }, true);
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
            var answer = submissionVm.Answers.FirstOrDefault(x => x.PageId == "your_contact_details" && x.QuestionId == "your_contact_details_01");
            if (answer != null)
            {
                var question = answer.Question;
                var fullName = answer.Answer;
                var email = string.Empty;
                var telNum = string.Empty;

                //contact details 2
                answer = submissionVm.Answers.FirstOrDefault(x => x.PageId == "your_contact_details" && x.QuestionId == "your_contact_details_02");
                if (answer != null) { email = answer.Answer; }
                //contact details 3
                answer = submissionVm.Answers.FirstOrDefault(x => x.PageId == "your_contact_details" && x.QuestionId == "your_contact_details_03");
                if (answer != null) { telNum = answer.Answer; }

                GetDataSection(body, question,
                    new List<string>
                    {
                        $"Full name: {fullName}",
                        $" Email address: {email}",
                        $" UK telephone number: {telNum}"
                    },
                    true
                );
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
            var record = submissionVm.Answers.FirstOrDefault(x => x.PageId == pageNum);
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
