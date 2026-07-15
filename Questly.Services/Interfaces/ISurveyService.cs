using Questly.Domain.Entities;
using Questly.Services.DTOs.Survey;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.Interfaces
{
    public interface ISurveyService
    {
        Task<DashboardDto> GetDashboardAsync(string userId, string? search, int page = 1, int pageSize = 2);

        Task<int> CreateSurveyAsync(CreateSurveyDto surveyDto);

        Task<GetSurveyDto?> GetSurveyByIdAsync(int id);

        Task<GetSurveyDto?> GetSurveyDetailedByIdAsync(int id);

        Task<List<GetSurveyDto>> GetUserSurveysAsync(string userId);

        Task<bool> UpdateSurveyAsync(UpdateSurveyDto surveyDto);

        Task<bool> DeleteSurveyAsync(int id);

        Task<TakeSurveyDto?> GetTakeSurveyDtoAsync(int surveyId);

        Task SubmitSurveyAsync(TakeSurveyDto takeSurveyDto, string? userId);

        Task<SurveyResultsDto?> GetSurveyResultsAsync(int surveyId);

        Task<int> CloneSurveyAsync(int surveyId, string userId);

        Task PublishSurveyAsync(int surveyId);

        Task UnpublishSurveyAsync(int surveyId);

        Task SetExpirationAsync(int surveyId, DateTime? closedAt);

        Task<TakeSurveyDto?> GetPublicSurveyAsync(Guid publicId);

        Task<bool> HasSkipLogic(int surveyId);
    }
}
