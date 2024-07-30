using Microsoft.AspNetCore.Mvc;
using Tengella.Survey.Business.Interfaces;
using Tengella.Survey.WebApp.Models;

namespace Tengella.Survey.WebApp.Controllers
{
    public class SurveyAnalysisController(IAnalysisService analysisService, ISurveyService surveyService) : Controller
    {
        private readonly IAnalysisService _analysisService = analysisService;
        private readonly ISurveyService _surveyService = surveyService;
        public async Task<IActionResult> SurveyAnalysis(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            var analysis = await _analysisService.AnalyzeSurveyAsync(id);
            return View(analysis);
        }

        public async Task<IActionResult> SurveySummary()
        {
            var repeatedQuestions = await _analysisService.GetLogsByTypeAsync("RepeatedQuestion");
            var surveyCompletions = await _analysisService.GetLogsByTypeAsync("SurveyCompletion");
            var emailSends = await _analysisService.GetLogsByTypeAsync("EmailSent");

            var model = new SurveySummaryViewModel
            {
                RepeatedQuestions = repeatedQuestions,
                SurveyCompletions = surveyCompletions,
                EmailSends = emailSends
            };

            return View(model);
        }
        public async Task<IActionResult> QuestionTrendAnalysis(int id)
        {
            var trendAnalysis = await _analysisService.GetQuestionTrendAnalysisAsync(id);
            return View(trendAnalysis);
        }
    }
}
