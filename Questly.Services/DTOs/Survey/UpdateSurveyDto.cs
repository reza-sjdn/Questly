using Questly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.DTOs.Survey
{
    public class UpdateSurveyDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; } = string.Empty;

        public ICollection<UpdateSurveyQuestionDto> Questions { get; set; } = new List<UpdateSurveyQuestionDto>();
    }
}
