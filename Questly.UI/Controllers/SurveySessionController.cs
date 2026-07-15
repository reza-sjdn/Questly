using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Questly.Domain.Entities;
using Questly.Services.DTOs.Question;
using Questly.Services.Interfaces;
using Questly.UI.Models.Question;
using System.Security.Claims;

namespace Questly.UI.Controllers
{
    public class SurveySessionController(ISurveySessionService _surveySessionService,
                                         IMapper _mapper) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Start(int surveyId)
        {
            var sessionKey = await _surveySessionService.StartSurveyAsync(
                surveyId,
                User.FindFirstValue(ClaimTypes.NameIdentifier));

            return RedirectToAction(nameof(Question), new { sessionKey });
        }

        [HttpGet]
        public async Task<IActionResult> Question(Guid sessionKey)
        {
            var dto = await _surveySessionService.GetCurrentQuestionAsync(sessionKey);

            if (dto == null)
                return NotFound();

            var model = _mapper.Map<TakeQuestionViewModel>(dto);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Answer(TakeQuestionViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Question", model);

            var dto = _mapper.Map<TakeQuestionDto>(model);

            var nextSessionKey = await _surveySessionService.SubmitAnswerAsync(dto);

            if (nextSessionKey == null)
                return RedirectToAction(nameof(Completed));

            return RedirectToAction(nameof(Question),
                new { sessionKey = nextSessionKey });
        }

        public async Task<IActionResult> Completed() => View();
    }
}
