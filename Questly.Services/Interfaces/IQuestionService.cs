using Questly.Domain.Entities;
using Questly.Services.DTOs.Question;
using Questly.Services.DTOs.Survey;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<int> CreateQuestionAsync(CreateQuestionDto questionDto);

        Task<GetQuestionDto?> GetQuestionByIdAsync(int id);

        Task<bool> UpdateQuestionAsync(UpdateQuestionDto questionDto);

        Task<bool> DeleteQuestionAsync(int id);

        Task ReorderQuestionsAsync(int surveyId, List<int> questionIds);
    }
}
