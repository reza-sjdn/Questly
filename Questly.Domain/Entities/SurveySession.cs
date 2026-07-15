using Questly.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Domain.Entities
{
    public class SurveySession : BaseEntity
    {
        public Guid SessionKey { get; set; } = Guid.NewGuid();

        public int SurveyId { get; set; }

        public int CurrentQuestionId { get; set; }

        public string? UserId { get; set; }

        public bool IsCompleted { get; set; }
    }
}
