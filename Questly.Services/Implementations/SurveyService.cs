using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Questly.Data.Context;
using Questly.Domain.Entities;
using Questly.Domain.Enums;
using Questly.Services.DTOs.Question;
using Questly.Services.DTOs.Survey;
using Questly.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.Implementations
{
    public class SurveyService(QuestlyDbContext _context, IMapper _mapper) : ISurveyService
    {
        public async Task<DashboardDto> GetDashboardAsync(string userId, string? search, int page = 1, int pageSize = 4)
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
            var survey = await _context.Surveys.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == surveyId);

            var questions = await _context.Questions.AsNoTracking()
                .Where(q => q.SurveyId == surveyId)
                .Include(q => q.Options.OrderBy(o => o.DisplayOrder))
                .Include(q => q.MatrixRows.OrderBy(o => o.DisplayOrder))
                .ToListAsync();

            survey.Questions = questions;

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
                if (question.Type == QuestionType.Matrix)
                {
                    foreach (var matrix in question.MatrixAnswers)
                    {
                        response.ResponseAnswers.Add(new ResponseAnswer
                        {
                            QuestionId = question.Id,
                            MatrixRowId = matrix.MatrixRowId,
                            ResponseAnswerOptions =
                            {
                                new ResponseAnswerOption
                                {
                                    QuestionOptionId = matrix.SelectedOptionId!.Value
                                }
                            }
                        });
                    }
                }

                else
                {
                    var answer = new ResponseAnswer
                    {
                        QuestionId = question.Id,
                        AnswerText = question.AnswerText,
                        FileName = question.FileName,
                        FilePath = question.FilePath,
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

            }

            _context.SurveyResponses.Add(response);

            await _context.SaveChangesAsync();
        }

        public async Task<SurveyResultsDto?> GetSurveyResultsAsync(int surveyId)
        {
            var survey = await _context.Surveys.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == surveyId);

            var questions = await _context.Questions.AsNoTracking()
                .Where(q => q.SurveyId == surveyId)
                .Include(q => q.MatrixRows.OrderBy(r => r.DisplayOrder))
                .Include(q => q.Options.OrderBy(r => r.DisplayOrder))
                .ToListAsync();

            survey.Questions = questions;

            if (survey == null)
                return null;

            var surveyResultDto = new SurveyResultsDto
            {
                SurveyId = survey.Id,
                SurveyTitle = survey.Title
            };

            foreach (var question in survey.Questions)
            {
                if (question.Type == QuestionType.SingleChoice ||
                    question.Type == QuestionType.MultipleChoice ||
                    question.Type == QuestionType.Dropdown)
                {
                    var questionResultDto = new QuestionResultDto
                    {
                        QuestionId = question.Id,
                        QuestionText = question.Text,
                        Type = question.Type
                    };

                    foreach (var option in question.Options)
                    {
                        var count = await _context.ResponseAnswerOptions
                            .CountAsync(rao => rao.QuestionOptionId == option.Id);

                        questionResultDto.Options.Add(new OptionResultDto
                        {
                            OptionId = option.Id,
                            OptionText = option.Text,
                            Count = count
                        });
                    }

                    // Calculate the Percentage
                    var totalVotes = questionResultDto.Options.Sum(o => o.Count);

                    foreach (var option in questionResultDto.Options)
                    {
                        option.Percentage = totalVotes == 0
                            ? 0
                            : option.Count * 100.0 / totalVotes;
                    }


                    surveyResultDto.Questions.Add(questionResultDto);
                }

                if (question.Type == QuestionType.Rating5 ||
                    question.Type == QuestionType.Rating10)
                {
                    var query = _context.ResponseAnswers.AsNoTracking()
                        .Where(ra => ra.QuestionId == question.Id);

                    var ratingResponseCount = await query.CountAsync(r => (r.AnswerText != null));
                    var ratingAvg = (double)(await query.SumAsync(ra => Convert.ToInt32(ra.AnswerText))) /
                                    ratingResponseCount;

                    var questionResultDto = new QuestionResultDto
                    {
                        QuestionId = question.Id,
                        QuestionText = question.Text,
                        Type = question.Type,
                        RatingAverage = ratingAvg,
                        ResponseCount = ratingResponseCount,
                    };

                    surveyResultDto.Questions.Add(questionResultDto);
                }

                if (question.Type == QuestionType.Matrix)
                {
                    var questionResultDto = new QuestionResultDto
                    {
                        QuestionId = question.Id,
                        QuestionText = question.Text,
                        Type = question.Type
                    };

                    questionResultDto.Options = question.Options
                        .OrderBy(c => c.DisplayOrder)
                        .Select(c => new OptionResultDto
                        {
                            OptionId = c.Id,
                            OptionText = c.Text
                        }).ToList();

                    foreach (var row in question.MatrixRows.OrderBy(r => r.DisplayOrder))
                    {
                        var rowDto = new MatrixRowResultDto
                        {
                            RowId = row.Id,
                            Text = row.Text
                        };

                        foreach (var option in question.Options.OrderBy(c => c.DisplayOrder))
                        {
                            var count = await _context.ResponseAnswers
                                .CountAsync(a =>
                                    a.MatrixRowId == row.Id &&
                                    a.ResponseAnswerOptions.Any(o => o.QuestionOptionId == option.Id));

                            rowDto.Results.Add(new MatrixCellResultDto
                            {
                                OptionId = option.Id,
                                Count = count
                            });
                        }

                        questionResultDto.MatrixRows.Add(rowDto);
                    }

                    surveyResultDto.Questions.Add(questionResultDto);
                }

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
                CreatedAt = DateTime.Now,
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

        public async Task<TakeSurveyDto?> GetPublicSurveyAsync(Guid publicId)
        {
            var survey = await _context.Surveys.SingleOrDefaultAsync(s => s.PublicId == publicId);
            if (survey == null) return null;
            return await GetTakeSurveyDtoAsync(survey.Id);
        }

        public async Task<Guid?> GetPublicIdOfSurveyAsync(int surveyId) =>
            (await GetSurveyByIdAsync(surveyId))?.PublicId;

        public async Task<bool> HasSkipLogic(int surveyId)
        {
            return _context.Questions
                .Where(q => q.SurveyId == surveyId)
                .SelectMany(q => q.Options)
                .Any(o => o.NextQuestionId.HasValue);
        }
    }
}
