using Questly.Domain.Enums;
using Questly.Services.DTOs.Survey;

namespace Questly.UI.Models.Question
{
    public class TakeQuestionViewModel
    {
        public Guid SessionKey { get; set; }

        public int QuestionId { get; set; }

        public QuestionType Type { get; set; }

        public bool IsRequired { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public List<TakeSurveyOptionDto> Options { get; set; } = new();

        public string? AnswerText { get; set; }

        public int? SelectedOptionId { get; set; }

        public List<int> SelectedOptionIds { get; set; } = new();
    }
}
