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
        }
    }
}
