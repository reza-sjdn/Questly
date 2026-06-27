using Questly.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.DTOs.Question
{
    public class UpdateQuestionDto
    {
        public int Id { get; set; }

        public int SurveyId { get; set; }

        public string Text { get; set; } = string.Empty;

        public QuestionType Type { get; set; }

        public bool IsRequired { get; set; }

        public List<string> Options { get; set; } = [];
    }
}
