using Questly.Services.DTOs;

namespace Questly.UI.Models.Survey
{
    public class GetSurveyViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string UserId { get; set; } = string.Empty;

        public List<GetQuestionViewModel> Questions { get; set; } = new List<GetQuestionViewModel>();
    }
}
