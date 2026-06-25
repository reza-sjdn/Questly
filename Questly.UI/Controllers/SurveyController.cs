using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Questly.Data.Entities;
using Questly.Domain.Entities;
using Questly.Services.DTOs;
using Questly.Services.Interfaces;
using Questly.UI.Models.Survey;

namespace Questly.UI.Controllers
{
    public class SurveyController(ISurveyService _surveyService,
                                  UserManager<ApplicationUser> _userManager,
                                  IMapper _mapper) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var model = await _surveyService.GetUserSurveysAsync(userId);
            return View(model);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Survey survey)
        {
            if (ModelState.IsValid)
            {
                survey.UserId = _userManager.GetUserId(User);
                int surveyId = await _surveyService.CreateSurveyAsync(survey);
                return RedirectToAction(nameof(Details), new { id = surveyId });
            }

            return View(survey);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            return View(survey);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Survey survey)
        {
            if (ModelState.IsValid)
            {
                await _surveyService.UpdateSurveyAsync(survey);
                return RedirectToAction(nameof(Index));
            }

            return View(survey);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _surveyService.DeleteSurveyAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var survey = await _surveyService.GetSurveyDetailedByIdAsync(id);
            return View(survey);
        }

        [HttpGet]
        public async Task<IActionResult> Take(int id)
        {
            var dto = await _surveyService.GetTakeSurveyDtoAsync(id);
            if (dto == null)
                return NotFound();
            var model = _mapper.Map<TakeSurveyViewModel>(dto);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(TakeSurveyViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Take", model);
            var dto = _mapper.Map<TakeSurveyDto>(model);
            await _surveyService.SubmitSurveyAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Results(int id)
        {
            var dto = await _surveyService.GetSurveyResultsAsync(id);
            if (dto == null)
                return NotFound();
            var model = _mapper.Map<SurveyResultsViewModel>(dto);
            return View(model);
        }

    }
}
