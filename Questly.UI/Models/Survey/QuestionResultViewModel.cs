using Questly.Domain.Enums;

namespace Questly.UI.Models.Survey
{
    public class QuestionResultViewModel
    {
        public int QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public QuestionType Type { get; set; }

        public double RatingAverage { get; set; }

        public int ResponseCount { get; set; }

        public List<OptionResultViewModel> Options { get; set; } = new();
    }
}
