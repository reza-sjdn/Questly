using Microsoft.EntityFrameworkCore;
using Questly.Data.Context;
using Questly.Domain.Entities;
using Questly.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Services.Implementations
{
    public class QuestionService(QuestlyDbContext _context) : IQuestionService
    {
        public async Task<int> CreateQuestionAsync(Question question)
        {
            var maxOrder = await _context.Questions
                .Where(q => q.SurveyId == question.SurveyId)
                .MaxAsync(q => (int?)q.DisplayOrder) ?? 0;

            question.DisplayOrder = maxOrder + 1;
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
            return question.Id;
        }

        public async Task<Question?> GetQuestionByIdAsync(int id) =>
            await _context.Questions.FindAsync(id);

        public async Task<List<Question>> GetSurveyQuestionsAsync(int surveyId) =>
            await _context.Questions.Where(q => q.Id == surveyId).ToListAsync();

        public async Task<bool> UpdateQuestionAsync(Question question)
        {
            _context.Questions.Update(question);
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
    }
}
