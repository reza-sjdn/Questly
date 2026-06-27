using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.DTOs.Survey
{
    public class GetQuestionOptionDto
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public string Text { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }
    }
}
