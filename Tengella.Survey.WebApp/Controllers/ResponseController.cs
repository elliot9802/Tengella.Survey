using Microsoft.AspNetCore.Mvc;
using Tengella.Survey.Business.Interfaces;
using Tengella.Survey.WebApp.Models;

namespace Tengella.Survey.WebApp.Controllers
{
    public class ResponseController(ILogger<ResponseController> logger, IResponseService responseService, ISurveyService surveyService) : Controller
    {
        private readonly ILogger<ResponseController> _logger = logger;
        private readonly IResponseService _responseService = responseService;
        private readonly ISurveyService _surveyService = surveyService;

        public async Task<IActionResult> RespondSurvey(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            if (survey.ClosingDate < DateTime.Today)
            {
                TempData["ErrorMessage"] = "This survey is closed and cannot be responded to.";
                return View("Error", new ErrorViewModel());
            }
            return View(survey);
        }

        [HttpPost]
        public async Task<IActionResult> RespondSurvey(int id, IFormCollection form)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            if (survey.ClosingDate < DateTime.Today)
            {
                TempData["ErrorMessage"] = "This survey is closed and cannot be responded to.";
                return View("Error", new ErrorViewModel());
            }

            var responses = await _responseService.CreateResponsesAsync(survey, form);
            if (!responses.Any())
            {
                TempData["ErrorMessage"] = "Your response is empty. Please fill out the survey before submitting.";
                return View(survey);
            }

            _logger.LogInformation("Responses: {Response}", responses.ToString());
            await _responseService.SaveResponsesAsync(responses);
            TempData["SuccessMessage"] = "Thank you for your response!";

            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
