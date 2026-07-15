using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Questly.Data.Entities;
using Questly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Questly.Data.Context
{
    public class QuestlyDbContext : IdentityDbContext<ApplicationUser>
    {
        public QuestlyDbContext(DbContextOptions<QuestlyDbContext> options)
            : base(options)
        {
        }

        public DbSet<Survey> Surveys => Set<Survey>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
        public DbSet<SurveyResponse> SurveyResponses => Set<SurveyResponse>();
        public DbSet<ResponseAnswer> ResponseAnswers => Set<ResponseAnswer>();
        public DbSet<ResponseAnswerOption> ResponseAnswerOptions => Set<ResponseAnswerOption>();
        public DbSet<SurveyTemplate> SurveyTemplates => Set<SurveyTemplate>();
        public DbSet<SurveyTemplateQuestion> SurveyTemplateQuestions => Set<SurveyTemplateQuestion>();
        public DbSet<SurveyTemplateOption> SurveyTemplateOptions => Set<SurveyTemplateOption>();
        public DbSet<SurveySession> SurveySessions => Set<SurveySession>();
        public DbSet<SurveySessionAnswer> SurveySessionAnswers => Set<SurveySessionAnswer>();


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ResponseAnswer>()
                .HasOne(q => q.SurveyResponse)
                .WithMany(s => s.ResponseAnswers)
                .HasForeignKey(q => q.SurveyResponseId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ResponseAnswerOption>()
                .HasOne(o => o.ResponseAnswer)
                .WithMany(q => q.ResponseAnswerOptions)
                .HasForeignKey(o => o.ResponseAnswerId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<QuestionOption>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
