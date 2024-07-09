using Microsoft.AspNetCore.Mvc;
using Tengella.Survey.Business.Interfaces;

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

            var responses = await _responseService.CreateResponsesAsync(survey, form);
            _logger.LogInformation("Responses: {Response}", responses.ToString());
            await _responseService.SaveResponsesAsync(responses);

            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
