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

namespace Questly.Services.Implementations
{
    public class QuestionService(QuestlyDbContext _context, IMapper _mapper) : IQuestionService
    {
        public async Task<int> CreateQuestionAsync(CreateQuestionDto questionDto)
        {
            var question = _mapper.Map<Question>(questionDto);
            var maxOrder = await _context.Questions
                .Where(q => q.SurveyId == questionDto.SurveyId)
                .MaxAsync(q => (int?)q.DisplayOrder) ?? 0;
            question.DisplayOrder = maxOrder + 1;
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
            return question.Id;
        }

        public async Task<GetQuestionDto?> GetQuestionByIdAsync(int id)
        {
            var question = await _context.Questions.Where(q => q.Id == id)
                .Include(q => q.Options)
                .FirstOrDefaultAsync();
            return _mapper.Map<GetQuestionDto>(question);
        }

        public async Task<bool> UpdateQuestionAsync(UpdateQuestionDto questionDto)
        {
            var question = await _context.Questions.FindAsync(questionDto.Id);
            _context.QuestionOptions.Where(o => o.QuestionId == questionDto.Id).ExecuteDelete();
            _mapper.Map(questionDto, question);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteQuestionAsync(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return false;
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ReorderQuestionsAsync(int surveyId, List<int> questionIds)
        {
            var questions = await _context.Questions
                .Where(q => q.SurveyId == surveyId)
                .ToListAsync();

            for (int i = 0; i < questionIds.Count; i++)
            {
                var question = questions.First(q => q.Id == questionIds[i]);
                question.DisplayOrder = i + 1;
            }

            await _context.SaveChangesAsync();
        }
    }
}
