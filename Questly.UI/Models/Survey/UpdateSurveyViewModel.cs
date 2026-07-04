namespace Questly.UI.Models.Survey
{
    public class UpdateSurveyViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsPublished { get; set; }

        public DateTime? PublishedAt { get; set; }

        public string UserId { get; set; } = string.Empty;

        public bool AllowAnonymousResponses { get; set; } = true;
    }
}
