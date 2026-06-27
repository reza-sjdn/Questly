namespace Questly.UI.Models.Survey
{
    public class GetQuestionOptionViewModel
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public string Text { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }
    }
}
