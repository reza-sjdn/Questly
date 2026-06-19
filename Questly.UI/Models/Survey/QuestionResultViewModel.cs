namespace Questly.UI.Models.Survey
{
    public class QuestionResultViewModel
    {
        public int QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public List<OptionResultViewModel> Options { get; set; } = new();
    }
}
