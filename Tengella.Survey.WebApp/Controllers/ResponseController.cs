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
                TempData["ErrorMessage"] = "Denna enkät är stängd och kan inte besvaras.";
                return View("Error", new ErrorViewModel());
            }
            
            var cachedResponses = _responseService.GetCachedResponses(survey.SurveyFormId);
            ViewBag.CachedResponses = cachedResponses;

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
                TempData["ErrorMessage"] = "Denna enkät är stängd och kan inte besvaras.";
                return View("Error", new ErrorViewModel());
            }

            var responses = await _responseService.CreateResponsesAsync(survey, form);
            if (!responses.Any())
            {
                TempData["ErrorMessage"] = "Du kan inte skicka in en enkät utan några svar.";
                return View(survey);
            }

            _logger.LogInformation("Responses: {Response}", responses.ToString());
            await _responseService.SaveResponsesAsync(responses);
            TempData["SuccessMessage"] = "Tack för dina svar!";

            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
