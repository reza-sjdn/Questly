using Questly.Domain.Enums;

namespace Questly.UI.Models.Survey
{
    public class TakeSurveyQuestionViewModel
    {
        public int QuestionId { get; set; }

        public string Text { get; set; } = string.Empty;

        public QuestionType Type { get; set; }

        public bool IsRequired { get; set; }

        // For ShortText / LongText
        public string? AnswerText { get; set; }

        // For SingleChoice / Dropdown
        public int? SelectedOptionId { get; set; }

        // For MultipleChoice
        public List<int> SelectedOptionIds { get; set; } = new();

        public List<TakeSurveyOptionViewModel> Options { get; set; } = new();
    }
}
