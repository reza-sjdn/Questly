using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.DTOs.Survey
{
    public class OptionResultDto
    {
        public int OptionId { get; set; }

        public string OptionText { get; set; } = string.Empty;

        public int Count { get; set; }
    }
}
