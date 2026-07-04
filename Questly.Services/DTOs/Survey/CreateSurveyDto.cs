using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.DTOs.Survey
{
    public class CreateSurveyDto
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; } = string.Empty;

        public bool AllowAnonymousResponses { get; set; } = true;

        public ICollection<CreateSurveyQuestionDto> Questions { get; set; } = new List<CreateSurveyQuestionDto>();
    }
}
