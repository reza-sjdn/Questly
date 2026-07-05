using Questly.Services.DTOs;

namespace Questly.UI.Models.Survey
{
    public class GetSurveyViewModel
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

        public List<GetQuestionViewModel> Questions { get; set; } = new List<GetQuestionViewModel>();
    }
}
