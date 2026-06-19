using Questly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<int> CreateQuestionAsync(Question question);

        Task<Question?> GetQuestionByIdAsync(int id);

        Task<List<Question>> GetSurveyQuestionsAsync(int surveyId);

        Task<bool> UpdateQuestionAsync(Question question);

        Task<bool> DeleteQuestionAsync(int id);
    }
}
