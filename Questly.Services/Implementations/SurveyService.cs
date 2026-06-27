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
        public async Task<int> CreateSurveyAsync(CreateSurveyDto surveyDto)
        {
            var survey = _mapper.Map<Survey>(surveyDto);
            await _context.Surveys.AddAsync(survey);
            await _context.SaveChangesAsync();
            return survey.Id;
        }

        public async Task<GetSurveyDto?> GetSurveyByIdAsync(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
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

        public async Task SubmitSurveyAsync(TakeSurveyDto takeSurveyDto)
        {
            var response = new SurveyResponse
            {
                SurveyId = takeSurveyDto.Id,
                SubmittedAt = DateTime.Now
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

                surveyResultDto.Questions.Add(questionDto);
            }

            return surveyResultDto;
        }

    }
}
