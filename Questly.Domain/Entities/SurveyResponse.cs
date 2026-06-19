using Questly.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Domain.Entities
{
    public class SurveyResponse : BaseEntity
    {
        public int SurveyId { get; set; }

        public DateTime SubmittedAt { get; set; }

        public Survey Survey { get; set; } = null!;

        public ICollection<ResponseAnswer> ResponseAnswers { get; set; } = new List<ResponseAnswer>();
    }
}
