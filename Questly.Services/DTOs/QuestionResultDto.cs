using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.DTOs
{
    public class QuestionResultDto
    {
        public int QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public List<OptionResultDto> Options { get; set; } = new();
    }
}
