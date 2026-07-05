using Questly.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Questly.Domain.Entities
{
    public class Survey : BaseEntity
    {
        public Guid PublicId { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsPublished { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        [NotMapped]
        public bool IsOpen =>
            !ClosedAt.HasValue || ClosedAt > DateTime.Now;

        [NotMapped]
        public bool IsAvailable =>
            IsPublished &&
            (!ClosedAt.HasValue || ClosedAt > DateTime.Now);

        public string UserId { get; set; } = string.Empty;

        public bool AllowAnonymousResponses { get; set; } = false;

        public ICollection<Question> Questions { get; set; } = new List<Question>();

        public ICollection<SurveyResponse> SurveyResponses { get; set; } = new List<SurveyResponse>();
    }
}
