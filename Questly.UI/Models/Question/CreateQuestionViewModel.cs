using Questly.Domain.Enums;

namespace Questly.UI.Models.Question
{
    public class CreateQuestionViewModel
    {
        public int SurveyId { get; set; }

        public string Text { get; set; } = string.Empty;

        public QuestionType Type { get; set; }

        public bool IsRequired { get; set; }

        public List<string> Options { get; set; } = [];
    }
}
