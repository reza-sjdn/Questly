namespace Questly.UI.Models.Survey
{
    public class SurveyResultsViewModel
    {
        public int SurveyId { get; set; }

        public string SurveyTitle { get; set; } = string.Empty;

        public List<QuestionResultViewModel> Questions { get; set; } = new();
    }
}
