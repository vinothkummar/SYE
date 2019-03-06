using System.Linq;
using GDSHelpers.Models.FormSchema;
using GDSHelpers.Models.SubmissionSchema;
using Microsoft.AspNetCore.Http;

namespace GDSHelpers
{
    public interface IGdsValidation
    {
        /// <summary>
        /// Validates a PageVM object against the Request.Form object after postback
        /// </summary>
        /// <param name="pageVm">A PageVM object containing the current page.</param>
        /// <param name="requestForm">The Request.Form object after postback of the current page.</param>
        /// <returns>A validated PageVM object with error flags and messages marked against each question.</returns>
        PageVM ValidatePage(PageVM pageVm, IFormCollection requestForm);

        /// <summary>
        /// Builds a SubmissionVM from the current pageVM
        /// </summary>
        /// <param name="pagevm">The current PageVM</param>
        /// <returns>Returns a SubmissionVM model ready to be serialized and stored in a DB.</returns>
        SubmissionVM BuildSubmission(PageVM pagevm);
    }

    public class GdsValidation : IGdsValidation
    {
        public PageVM ValidatePage(PageVM pageVm, IFormCollection requestForm)
        {
            foreach (var question in pageVm.Questions)
            {
                //Get the answer
                var answer = requestForm[question.QuestionId].ToString();
                question.Answer = answer;


                //Set the next page id if our answer matches a rule
                var nextPageId = question.AnswerLogic?.FirstOrDefault(m => m.Value == answer)?.NextPageId;
                if(nextPageId != null) pageVm.NextPageId = nextPageId;


                //Check if question is required
                if (question.Validation?.Required.IsRequired == true && string.IsNullOrEmpty(answer))
                {
                    question.Validation.IsErrored = true;
                    question.Validation.ErrorMessage = question.Validation.Required.ErrorMessage;
                }


                //Check length
                if (question.Validation?.MaxLength?.Max < answer?.Length)
                {
                    question.Validation.IsErrored = true;
                    question.Validation.ErrorMessage = question.Validation.MaxLength.ErrorMessage;
                }


                //Check Minimum\Maximum Selected
                var selectedOptionsCount = answer.Split(',').Length;
                var min = question.Validation?.Selected?.Min;
                var max = question.Validation?.Selected?.Max;
                if (selectedOptionsCount < min || selectedOptionsCount > max)
                {
                    question.Validation.IsErrored = true;
                    question.Validation.ErrorMessage = question.Validation.Selected.ErrorMessage;
                }

            }

            return pageVm;
        }

        public SubmissionVM BuildSubmission(PageVM pageVm)
        {
            var answers = pageVm.Questions.Select(m =>
                new AnswerVM
                {
                    QuestionId = m.QuestionId,
                    Question = m.Question,
                    Answer = m.Answer
                });

            var submissionVm = new SubmissionVM { Answers = answers };

            return submissionVm;
        }

    }
}
