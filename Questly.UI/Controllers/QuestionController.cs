using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Questly.Domain.Entities;
using Questly.Services.DTOs.Question;
using Questly.Services.Implementations;
using Questly.Services.Interfaces;
using Questly.UI.Models.Question;
using Questly.UI.Models.Survey;

namespace Questly.UI.Controllers
{
    public class QuestionController(IQuestionService _questionService, IMapper _mapper) : Controller
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
        public async Task<IActionResult> Create(CreateQuestionViewModel surveyModel)
        {
            if (!ModelState.IsValid)
                return View(surveyModel);
            var surveyDto = _mapper.Map<CreateQuestionDto>(surveyModel);
            await _questionService.CreateQuestionAsync(surveyDto);
            return RedirectToAction("Details", "Survey",
                new { id = surveyModel.SurveyId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var surveyDto = await _questionService.GetQuestionByIdAsync(id);
            var question = _mapper.Map<GetQuestionViewModel>(surveyDto);
            return View(question);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateQuestionViewModel surveyModel)
        {
            if (ModelState.IsValid)
            {
                var questionDto = _mapper.Map<UpdateQuestionDto>(surveyModel);
                await _questionService.UpdateQuestionAsync(questionDto);
                return RedirectToAction("Details", "Survey", new { id = questionDto.SurveyId });
            }

            return View(nameof(Edit), new { surveyModel.Id });
        }

        public async Task<IActionResult> Delete(int id, int surveyId)
        {
            await _questionService.DeleteQuestionAsync(id);
            return RedirectToAction("Details", "Survey", new { id = surveyId });
        }

        [HttpPost]
        public async Task<IActionResult> Reorder(int surveyId, List<int> questionIds)
        {
            await _questionService.ReorderQuestionsAsync(surveyId, questionIds);
            return Ok();
        }
    }
}
