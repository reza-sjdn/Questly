using Questly.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.DTOs.Survey
{
    public class QuestionResultDto
    {
        public int QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public QuestionType Type { get; set; }

        public double RatingAverage { get; set; }

        public int ResponseCount { get; set; }

        public List<OptionResultDto> Options { get; set; } = new();
    }
}
