using Questly.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Domain.Entities
{
    public class ResponseAnswer : BaseEntity
    {
        public int SurveyResponseId { get; set; }

        public int QuestionId { get; set; }

        public string? AnswerText { get; set; }

        public SurveyResponse SurveyResponse { get; set; } = null!;

        public Question Question { get; set; } = null!;

        public ICollection<ResponseAnswerOption> ResponseAnswerOptions { get; set; } = new List<ResponseAnswerOption>();
    }
}
