using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.DTOs.Survey
{
    public class SurveyResultsDto
    {
        public int SurveyId { get; set; }

        public string SurveyTitle { get; set; } = string.Empty;

        public List<QuestionResultDto> Questions { get; set; } = new();
    }
}
