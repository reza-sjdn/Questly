using Microsoft.AspNetCore.Mvc;
using Questly.Domain.Entities;
using Questly.Services.Interfaces;
using Questly.UI.Models.Question;

namespace Questly.UI.Controllers
{
    public class QuestionController(IQuestionService _questionService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Create(int surveyId)
        {
            return View(new CreateQuestionViewModel
            {
                SurveyId = surveyId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateQuestionViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var question = new Question
            {
                SurveyId = model.SurveyId,
                Text = model.Text,
                Type = model.Type,
                IsRequired = model.IsRequired
            };

            question.Options = model.Options
           .Where(o => !string.IsNullOrWhiteSpace(o))
           .Select((o, index) => new QuestionOption
           {
               Text = o,
               DisplayOrder = index + 1
           })
           .ToList();

            await _questionService.CreateQuestionAsync(question);

            return RedirectToAction("Details", "Survey",
                new { id = model.SurveyId });
        }
    }
}
