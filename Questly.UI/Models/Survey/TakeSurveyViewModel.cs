namespace Questly.UI.Models.Survey
{
    public class TakeSurveyViewModel
    {
        public int SurveyId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public List<TakeSurveyQuestionViewModel> Questions { get; set; } = new();
    }
}
