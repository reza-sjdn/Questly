using Questly.Domain.Entities;
using Questly.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.Interfaces
{
    public interface ISurveyService
    {
        Task<int> CreateSurveyAsync(Survey survey);

        Task<Survey?> GetSurveyByIdAsync(int id);

        Task<Survey?> GetSurveyDetailedByIdAsync(int id);

        Task<List<Survey>> GetUserSurveysAsync(string userId);

        Task<bool> UpdateSurveyAsync(Survey survey);

        Task<bool> DeleteSurveyAsync(int id);

        Task<TakeSurveyDto?> GetTakeSurveyDtoAsync(int surveyId);

        Task SubmitSurveyAsync(TakeSurveyDto model);

        Task<SurveyResultsDto?> GetSurveyResultsAsync(int surveyId);
    }
}
