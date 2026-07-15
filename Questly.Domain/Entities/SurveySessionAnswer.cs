using Questly.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Domain.Entities
{
    public class SurveySessionAnswer : BaseEntity
    {
        public Guid SessionKey { get; set; }

        public int QuestionId { get; set; }

        public string? AnswerText { get; set; }

        public int? SelectedOptionId { get; set; }

        public string? SelectedOptionIdsJson { get; set; }
    }
}
