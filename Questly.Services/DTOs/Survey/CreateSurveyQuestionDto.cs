using Questly.Domain.Entities;
using Questly.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.DTOs.Survey
{
    public class CreateSurveyQuestionDto
    {
        public int SurveyId { get; set; }

        public string Text { get; set; } = string.Empty;

        public QuestionType Type { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public Questly.Domain.Entities.Survey Survey { get; set; } = null!;

        public ICollection<CreateQuestionOptionDto> Options { get; set; } = new List<CreateQuestionOptionDto>();
    }
}
