using Questly.Domain.Enums;
using Questly.Services.DTOs;

namespace Questly.UI.Models.Survey
{
    public class GetQuestionViewModel
    {
        public int Id { get; set; }

        public int SurveyId { get; set; }

        public string Text { get; set; } = string.Empty;

        public QuestionType Type { get; set; }

        public bool IsRequired { get; set; }

        public int DisplayOrder { get; set; }

        public List<GetQuestionOptionViewModel> Options { get; set; } = new List<GetQuestionOptionViewModel>();
    }
}
