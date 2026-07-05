using Questly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.DTOs.Survey
{
    public class GetSurveyDto
    {
        public int Id { get; set; }

        public Guid PublicId { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsPublished { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public bool IsOpen { get; set; }

        public string UserId { get; set; } = string.Empty;

        public bool AllowAnonymousResponses { get; set; } = true;

        public List<GetQuestionDto> Questions { get; set; } = new List<GetQuestionDto>();
    }
}
