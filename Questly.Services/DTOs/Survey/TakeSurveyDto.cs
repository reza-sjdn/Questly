using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.DTOs.Survey
{
    public class TakeSurveyDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsPublished { get; set; }

        public DateTime? PublishedAt { get; set; }

        public bool IsAvailable { get; set; }

        public bool AllowAnonymousResponses { get; set; } = true;

        public List<TakeSurveyQuestionDto> Questions { get; set; } = new();
    }
}
