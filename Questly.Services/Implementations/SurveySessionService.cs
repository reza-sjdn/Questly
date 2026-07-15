using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Questly.Data.Context;
using Questly.Domain.Entities;
using Questly.Services.DTOs.Question;
using Questly.Services.DTOs.Survey;
using Questly.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Questly.Services.Implementations
{
    public class SurveySessionService(QuestlyDbContext _context,
                                      IMapper _mapper) : ISurveySessionService
    {
        public async Task<Guid> StartSurveyAsync(int surveyId, string? userId)
        {
            var firstQuestion = await _context.Questions
                .Where(q => q.SurveyId == surveyId)
                .OrderBy(q => q.DisplayOrder)
                .FirstAsync();

            var session = new SurveySession
            {
                SurveyId = surveyId,
                CurrentQuestionId = firstQuestion.Id,
                UserId = userId
            };

            _context.SurveySessions.Add(session);
            await _context.SaveChangesAsync();

            return session.SessionKey;
        }

        public async Task<TakeQuestionDto?> GetCurrentQuestionAsync(Guid sessionKey)
        {
            var session = await _context.SurveySessions
                .FirstOrDefaultAsync(s => s.SessionKey == sessionKey);

            if (session == null)
                return null;

            var question = await _context.Questions
                .Include(q => q.Options.OrderBy(o => o.DisplayOrder))
                .FirstAsync(q => q.Id == session.CurrentQuestionId);

            var takeQuestionDto = _mapper.Map<TakeQuestionDto>(question);
            takeQuestionDto.SessionKey = sessionKey;
            return takeQuestionDto;
        }

        public async Task<Guid?> SubmitAnswerAsync(TakeQuestionDto dto)
        {
            var session = await _context.SurveySessions
                .FirstAsync(s => s.SessionKey == dto.SessionKey);

            var surveySessionAnswer = _mapper.Map<SurveySessionAnswer>(dto);
            _context.SurveySessionAnswers.Add(surveySessionAnswer);
            await _context.SaveChangesAsync();

            int? nextQuestionId = null;

            if (dto.SelectedOptionId.HasValue)
            {
                nextQuestionId = await _context.QuestionOptions
                    .Where(o => o.Id == dto.SelectedOptionId)
                    .Select(o => o.NextQuestionId)
                    .FirstAsync();
            }

            // If nextQuestionId is null and there is no branch, find next question by DisplayOrder
            if (nextQuestionId == null)
            {
                var current = await _context.Questions
                    .FirstAsync(q => q.Id == dto.QuestionId);

                nextQuestionId = await _context.Questions
                    .Where(q => q.SurveyId == session.SurveyId &&
                                q.DisplayOrder > current.DisplayOrder)
                    .OrderBy(q => q.DisplayOrder)
                    .Select(q => (int?)q.Id)
                    .FirstOrDefaultAsync();
            }

            // If after previous block, nextQuestionId is still null, Survey is finished
            if (nextQuestionId == null)
            {
                // Convert SurveySessionAnswers -> SurveyResponse
                // Delete SurveySession & SurveySessionAnswers
                var tempAnswers = await _context.SurveySessionAnswers
                    .Where(a => a.SessionKey == dto.SessionKey)
                    .ToListAsync();

                var response = new SurveyResponse
                {
                    SurveyId = session.SurveyId,
                    UserId = session.UserId,
                    SubmittedAt = DateTime.Now
                };

                foreach (var temp in tempAnswers)
                {
                    var answer = new ResponseAnswer
                    {
                        QuestionId = temp.QuestionId,
                        AnswerText = temp.AnswerText
                    };

                    if (temp.SelectedOptionId.HasValue)
                    {
                        answer.ResponseAnswerOptions.Add(new ResponseAnswerOption
                        {
                            QuestionOptionId = temp.SelectedOptionId.Value
                        });
                    }

                    if (!string.IsNullOrEmpty(temp.SelectedOptionIdsJson))
                    {
                        var ids = JsonSerializer.Deserialize<List<int>>(temp.SelectedOptionIdsJson)!;

                        foreach (var id in ids)
                        {
                            answer.ResponseAnswerOptions.Add(new ResponseAnswerOption
                            {
                                QuestionOptionId = id
                            });
                        }
                    }

                    response.ResponseAnswers.Add(answer);
                }

                _context.SurveyResponses.Add(response);

                _context.SurveySessionAnswers.RemoveRange(tempAnswers);
                _context.SurveySessions.Remove(session);

                await _context.SaveChangesAsync();

                return null;
            }

            session.CurrentQuestionId = nextQuestionId.Value;
            await _context.SaveChangesAsync();
            return session.SessionKey;
        }
    }
}
