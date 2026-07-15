using Questly.Services.DTOs.Question;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.Interfaces
{
    public interface ISurveySessionService
    {
        Task<Guid> StartSurveyAsync(int surveyId, string? userId);

        Task<TakeQuestionDto?> GetCurrentQuestionAsync(Guid sessionKey);

        Task<Guid?> SubmitAnswerAsync(TakeQuestionDto dto);
    }
}
