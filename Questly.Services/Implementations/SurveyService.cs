using Microsoft.EntityFrameworkCore;
using Questly.Data.Context;
using Questly.Domain.Entities;
using Questly.Domain.Enums;
using Questly.Services.DTOs;
using Questly.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.Implementations
{
    public class SurveyService(QuestlyDbContext _context) : ISurveyService
    {
        public async Task<int> CreateSurveyAsync(Survey survey)
        {
            await _context.Surveys.AddAsync(survey);
            await _context.SaveChangesAsync();
            return survey.Id;
        }

        public async Task<Survey?> GetSurveyByIdAsync(int id) =>
            await _context.Surveys.FindAsync(id);

        public async Task<Survey?> GetSurveyDetailedByIdAsync(int id)
        {
            var survey = await _context.Surveys.AsNoTracking()
                .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(s => s.Id == id);

            survey.Questions = survey.Questions
                .OrderBy(q => q.DisplayOrder)
                .Select(q =>
                {
                    q.Options = q.Options.OrderBy(o => o.DisplayOrder).ToList();
                    return q;
                })
                .ToList();

            return survey;
        }

        public async Task<List<Survey>> GetUserSurveysAsync(string userId) =>
            await _context.Surveys.Where(s => s.UserId == userId).ToListAsync();

        public async Task<bool> UpdateSurveyAsync(Survey survey)
        {
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

            // Map Survey -> TakeSurveyDto
            var dto = new TakeSurveyDto
            {
                SurveyId = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Questions = survey.Questions
                    .OrderBy(q => q.DisplayOrder)
                    .Select(q => new TakeSurveyQuestionDto
                    {
                        QuestionId = q.Id,
                        Text = q.Text,
                        Type = q.Type,
                        IsRequired = q.IsRequired,
                        Options = q.Options
                            .OrderBy(o => o.DisplayOrder)
                            .Select(o => new TakeSurveyOptionDto
                            {
                                OptionId = o.Id,
                                Text = o.Text
                            })
                            .ToList()
                    })
                    .ToList()
            };

            return dto;
        }

        public async Task SubmitSurveyAsync(TakeSurveyDto dto)
        {
            var response = new SurveyResponse
            {
                SurveyId = dto.SurveyId,
                SubmittedAt = DateTime.Now
            };

            foreach (var question in dto.Questions)
            {
                var answer = new ResponseAnswer
                {
                    QuestionId = question.QuestionId,
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
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(s => s.Id == surveyId);

            if (survey == null)
                return null;

            var dto = new SurveyResultsDto
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

                foreach (var option in question.Options.OrderBy(o => o.DisplayOrder))
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

                dto.Questions.Add(questionDto);
            }

            return dto;
        }
    }
}
