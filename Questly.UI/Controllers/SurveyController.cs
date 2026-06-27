using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Questly.Data.Entities;
using Questly.Domain.Entities;
using Questly.Services.DTOs.Survey;
using Questly.Services.Interfaces;
using Questly.UI.Models.Survey;
using System.Security.Claims;

namespace Questly.UI.Controllers
{
    public class SurveyController(ISurveyService _surveyService,
                                  UserManager<ApplicationUser> _userManager,
                                  IMapper _mapper) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var surveyDto = await _surveyService.GetUserSurveysAsync(userId);
            var surveyModel = _mapper.Map<List<GetSurveyViewModel>>(surveyDto);
            return View(surveyModel);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateSurveyViewModel surveyModel)
        {
            if (ModelState.IsValid)
            {
                var surveyDto = _mapper.Map<CreateSurveyDto>(surveyModel);
                surveyDto.UserId = _userManager.GetUserId(User);
                int surveyId = await _surveyService.CreateSurveyAsync(surveyDto);
                return RedirectToAction(nameof(Details), new { id = surveyId });
            }

            return View(surveyModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var surveyDto = await _surveyService.GetSurveyByIdAsync(id);
            var surveyModel = _mapper.Map<GetSurveyViewModel>(surveyDto);
            return View(surveyModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateSurveyViewModel surveyModel)
        {
            if (ModelState.IsValid)
            {
                var surveyDto = _mapper.Map<UpdateSurveyDto>(surveyModel);
                await _surveyService.UpdateSurveyAsync(surveyDto);
                return RedirectToAction(nameof(Index));
            }

            return View(surveyModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _surveyService.DeleteSurveyAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var surveyDto = await _surveyService.GetSurveyDetailedByIdAsync(id);
            var surveyModel = _mapper.Map<GetSurveyViewModel>(surveyDto);
            return View(surveyModel);
        }

        [HttpGet]
        public async Task<IActionResult> Take(int id)
        {
            var surveyDto = await _surveyService.GetTakeSurveyDtoAsync(id);
            if (surveyDto == null)
                return NotFound();
            var surveyModel = _mapper.Map<TakeSurveyViewModel>(surveyDto);
            return View(surveyModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(TakeSurveyViewModel takeSurveyModel)
        {
            if (!ModelState.IsValid)
                return View("Take", takeSurveyModel);
            var takeSurveyDto = _mapper.Map<TakeSurveyDto>(takeSurveyModel);
            await _surveyService.SubmitSurveyAsync(takeSurveyDto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Results(int id)
        {
            var surveyResultsDto = await _surveyService.GetSurveyResultsAsync(id);
            if (surveyResultsDto == null)
                return NotFound();
            var surveyResultsModel = _mapper.Map<SurveyResultsViewModel>(surveyResultsDto);
            return View(surveyResultsModel);
        }

    }
}
