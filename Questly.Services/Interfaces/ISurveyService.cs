using Questly.Domain.Entities;
using Questly.Services.DTOs.Survey;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.Interfaces
{
    public interface ISurveyService
    {
        Task<int> CreateSurveyAsync(CreateSurveyDto surveyDto);

        Task<GetSurveyDto?> GetSurveyByIdAsync(int id);

        Task<GetSurveyDto?> GetSurveyDetailedByIdAsync(int id);

        Task<List<GetSurveyDto>> GetUserSurveysAsync(string userId);

        Task<bool> UpdateSurveyAsync(UpdateSurveyDto surveyDto);

        Task<bool> DeleteSurveyAsync(int id);

        Task<TakeSurveyDto?> GetTakeSurveyDtoAsync(int surveyId);

        Task SubmitSurveyAsync(TakeSurveyDto takeSurveyDto);

        Task<SurveyResultsDto?> GetSurveyResultsAsync(int surveyId);

    }
}
