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
        public async Task<IActionResult> Dashboard(string? search, int page = 1)
        {
            var userId = _userManager.GetUserId(User);
            var dashboardDto = await _surveyService.GetDashboardAsync(userId, search, page);
            var dashboardModel = _mapper.Map<DashboardViewModel>(dashboardDto);
            dashboardModel.Search = search;
            return View(dashboardModel);
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
                return RedirectToAction(nameof(Dashboard));
            }

            return View(surveyModel);
        }

        public async Task<IActionResult> Delete(int id)
        {
            await _surveyService.DeleteSurveyAsync(id);
            return RedirectToAction(nameof(Dashboard));
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

            if (!surveyDto.IsAvailable)
                return View("SurveyUnavailable");

            if (!surveyDto.AllowAnonymousResponses &&
                !User.Identity!.IsAuthenticated)
            {
                return Challenge();
            }

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

            string? userId = User.Identity!.IsAuthenticated
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

            await _surveyService.SubmitSurveyAsync(takeSurveyDto, userId);
            return RedirectToAction(nameof(Dashboard));
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

        [HttpPost]
        public async Task<IActionResult> Clone(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var newSurveyId = await _surveyService.CloneSurveyAsync(id, userId);

            return RedirectToAction(nameof(Details), new { id = newSurveyId });
        }

        [HttpPost]
        public async Task<IActionResult> Publish(int id)
        {
            await _surveyService.PublishSurveyAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Unpublish(int id)
        {
            await _surveyService.UnpublishSurveyAsync(id);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> SetExpiration(int id, DateTime? closedAt)
        {
            await _surveyService.SetExpirationAsync(id, closedAt);
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
