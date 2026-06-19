using Questly.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Domain.Entities
{
    public class Survey : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; } = string.Empty;

        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
