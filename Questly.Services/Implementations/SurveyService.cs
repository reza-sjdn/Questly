using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Questly.Data.Context;
using Questly.Domain.Entities;
using Questly.Domain.Enums;
using Questly.Services.DTOs.Survey;
using Questly.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.Implementations
{
    public class SurveyService(QuestlyDbContext _context, IMapper _mapper) : ISurveyService
    {
        public async Task<DashboardDto> GetDashboardAsync(string userId, string? search, int page = 1, int pageSize = 2)
        {
            var query = _context.Surveys.AsNoTracking()
                .Where(s => s.UserId == userId);

            // Apply the search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(s =>
                    s.Title.Contains(search) ||
                    (s.Description != null && s.Description.Contains(search)));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            DashboardDto dashboardDto = new()
            {
                TotalSurveys = await query.CountAsync(),
                PublishedSurveys = await query.CountAsync(s => s.IsPublished),
                DraftSurveys = await query.CountAsync(s => !s.IsPublished),
                TotalResponses = await query.SumAsync(s => s.SurveyResponses.Count()),
                Surveys = await query
                    .OrderByDescending(s => s.CreatedAt)
                    // Apply the pagination
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(s => new DashboardSurveyItemDto
                    {
                        Id = s.Id,
                        Title = s.Title,
                        IsPublished = s.IsPublished,
                        QuestionCount = s.Questions.Count(),
                        ResponseCount = s.SurveyResponses.Count(),
                        CreatedAt = s.CreatedAt
                    })
                    .ToListAsync()
            };

            dashboardDto.CurrentPage = page;
            dashboardDto.TotalPages = totalPages;
            dashboardDto.PageSize = pageSize;

            return dashboardDto;
        }

        public async Task<int> CreateSurveyAsync(CreateSurveyDto surveyDto)
        {
            var survey = _mapper.Map<Survey>(surveyDto);
            await _context.Surveys.AddAsync(survey);
            await _context.SaveChangesAsync();
            return survey.Id;
        }

        public async Task<GetSurveyDto?> GetSurveyByIdAsync(int id)
        {
            var survey = await _context.Surveys.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            var surveyDto = _mapper.Map<GetSurveyDto?>(survey);
            return surveyDto;
        }

        public async Task<GetSurveyDto?> GetSurveyDetailedByIdAsync(int id)
        {
            var survey = await _context.Surveys.AsNoTracking()
                .Include(s => s.Questions.OrderBy(q => q.DisplayOrder))
                    .ThenInclude(q => q.Options.OrderBy(o => o.DisplayOrder))
                .FirstOrDefaultAsync(s => s.Id == id);
            var surveyDto = _mapper.Map<GetSurveyDto>(survey);
            return surveyDto;
        }

        public async Task<List<GetSurveyDto>> GetUserSurveysAsync(string userId)
        {
            var surveys = await _context.Surveys.Where(s => s.UserId == userId).ToListAsync();
            var surveysDto = _mapper.Map<List<GetSurveyDto>>(surveys);
            return surveysDto;
        }

        public async Task<bool> UpdateSurveyAsync(UpdateSurveyDto surveyDto)
        {
            var survey = _mapper.Map<Survey>(surveyDto);
            _context.Surveys.Update(survey);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSurveyAsync(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null) return false;
            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TakeSurveyDto?> GetTakeSurveyDtoAsync(int surveyId)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions.OrderBy(q => q.DisplayOrder))
                    .ThenInclude(q => q.Options.OrderBy(o => o.DisplayOrder))
                .FirstOrDefaultAsync(s => s.Id == surveyId);

            if (survey == null)
                return null;

            var surveyDto = _mapper.Map<TakeSurveyDto>(survey);
            return surveyDto;
        }

        public async Task SubmitSurveyAsync(TakeSurveyDto takeSurveyDto, string? userId)
        {
            var response = new SurveyResponse
            {
                SurveyId = takeSurveyDto.Id,
                SubmittedAt = DateTime.Now,
                UserId = userId
            };

            foreach (var question in takeSurveyDto.Questions)
            {
                var answer = new ResponseAnswer
                {
                    QuestionId = question.Id,
                    AnswerText = question.AnswerText
                };

                foreach (var optionId in question.SelectedOptionIds)
                {
                    answer.ResponseAnswerOptions.Add(new ResponseAnswerOption
                    {
                        QuestionOptionId = optionId
                    });
                }

                if (question.SelectedOptionId.HasValue)
                {
                    answer.ResponseAnswerOptions.Add(new ResponseAnswerOption
                    {
                        QuestionOptionId = question.SelectedOptionId.Value
                    });
                }

                response.ResponseAnswers.Add(answer);
            }

            _context.SurveyResponses.Add(response);

            await _context.SaveChangesAsync();
        }

        public async Task<SurveyResultsDto?> GetSurveyResultsAsync(int surveyId)
        {
            var survey = await _context.Surveys.AsNoTracking()
                .Include(s => s.Questions.OrderBy(q => q.DisplayOrder))
                    .ThenInclude(q => q.Options.OrderBy(o => o.DisplayOrder))
                .FirstOrDefaultAsync(s => s.Id == surveyId);

            if (survey == null)
                return null;

            var surveyResultDto = new SurveyResultsDto
            {
                SurveyId = survey.Id,
                SurveyTitle = survey.Title
            };

            foreach (var question in survey.Questions.Where(q =>
                         q.Type == QuestionType.SingleChoice ||
                         q.Type == QuestionType.MultipleChoice ||
                         q.Type == QuestionType.Dropdown))
            {
                var questionDto = new QuestionResultDto
                {
                    QuestionId = question.Id,
                    QuestionText = question.Text
                };

                foreach (var option in question.Options)
                {
                    var count = await _context.ResponseAnswerOptions
                        .CountAsync(rao => rao.QuestionOptionId == option.Id);

                    questionDto.Options.Add(new OptionResultDto
                    {
                        OptionId = option.Id,
                        OptionText = option.Text,
                        Count = count
                    });
                }

                // Calculate the Percentage
                var totalVotes = questionDto.Options.Sum(o => o.Count);

                foreach (var option in questionDto.Options)
                {
                    option.Percentage = totalVotes == 0
                        ? 0
                        : option.Count * 100.0 / totalVotes;
                }


                surveyResultDto.Questions.Add(questionDto);
            }

            return surveyResultDto;
        }

        public async Task<int> CloneSurveyAsync(int surveyId, string userId)
        {
            var survey = await _context.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(s => s.Id == surveyId);

            if (survey == null)
                throw new Exception("Survey not found.");

            var clone = new Survey
            {
                Title = survey.Title + " (Copy)",
                Description = survey.Description,
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            foreach (var question in survey.Questions.OrderBy(q => q.DisplayOrder))
            {
                var newQuestion = new Question
                {
                    Text = question.Text,
                    Type = question.Type,
                    IsRequired = question.IsRequired,
                    DisplayOrder = question.DisplayOrder
                };

                foreach (var option in question.Options.OrderBy(o => o.DisplayOrder))
                {
                    newQuestion.Options.Add(new QuestionOption
                    {
                        Text = option.Text,
                        DisplayOrder = option.DisplayOrder
                    });
                }

                clone.Questions.Add(newQuestion);
            }

            _context.Surveys.Add(clone);

            await _context.SaveChangesAsync();

            return clone.Id;
        }

        public async Task PublishSurveyAsync(int surveyId)
        {
            var surveyDto = await GetSurveyByIdAsync(surveyId);
            surveyDto.IsPublished = true;
            surveyDto.PublishedAt = DateTime.Now;
            var updatedSurveyDto = _mapper.Map<UpdateSurveyDto>(surveyDto);
            await UpdateSurveyAsync(updatedSurveyDto);
        }

        public async Task UnpublishSurveyAsync(int surveyId)
        {
            var surveyDto = await GetSurveyByIdAsync(surveyId);
            surveyDto.IsPublished = false;
            var updatedSurveyDto = _mapper.Map<UpdateSurveyDto>(surveyDto);
            await UpdateSurveyAsync(updatedSurveyDto);
        }

        public async Task SetExpirationAsync(int surveyId, DateTime? closedAt)
        {
            var surveyDto = await GetSurveyByIdAsync(surveyId);
            if (surveyDto == null)
                throw new Exception("Survey not found.");
            surveyDto.ClosedAt = closedAt;
            var updatedSurveyDto = _mapper.Map<UpdateSurveyDto>(surveyDto);
            await UpdateSurveyAsync(updatedSurveyDto);
        }
    }
}
