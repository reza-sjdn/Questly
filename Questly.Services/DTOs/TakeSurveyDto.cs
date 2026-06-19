using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.DTOs
{
    public class TakeSurveyDto
    {
        public int SurveyId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public List<TakeSurveyQuestionDto> Questions { get; set; } = new();
    }
}
