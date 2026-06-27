namespace Questly.UI.Models.Survey
{
    public class UpdateSurveyViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; } = string.Empty;
    }
}
