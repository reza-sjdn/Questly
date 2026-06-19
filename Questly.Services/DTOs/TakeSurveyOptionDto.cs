using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.DTOs
{
    public class TakeSurveyOptionDto
    {
        public int OptionId { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
