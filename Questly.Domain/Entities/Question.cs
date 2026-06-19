using Questly.Domain.Common;
using Questly.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Domain.Entities
{
    public class Question : BaseEntity
    {
        public int SurveyId { get; set; }

        public string Text { get; set; } = string.Empty;

        public QuestionType Type { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public Survey Survey { get; set; } = null!;

        public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }
}
