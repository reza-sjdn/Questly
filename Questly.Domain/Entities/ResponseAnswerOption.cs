using Questly.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Domain.Entities
{
    public class ResponseAnswerOption : BaseEntity
    {
        public int ResponseAnswerId { get; set; }

        public int QuestionOptionId { get; set; }

        public ResponseAnswer ResponseAnswer { get; set; } = null!;

        public QuestionOption QuestionOption { get; set; } = null!;
    }
}
