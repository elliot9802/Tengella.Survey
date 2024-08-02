using Microsoft.AspNetCore.Mvc;
using Tengella.Survey.Business.Interfaces;
using Tengella.Survey.Business.Services;
using Tengella.Survey.WebApp.Models;

namespace Tengella.Survey.WebApp.Controllers
{
    public class SurveyAnalysisController(IAnalysisService analysisService, ISurveyService surveyService) : Controller
    {
        private readonly IAnalysisService _analysisService = analysisService;
        private readonly ISurveyService _surveyService = surveyService;
        public async Task<IActionResult> SurveyAnalysis(int id)
        {
            var analysis = await _analysisService.GetSurveyResponseAnalysisAsync(id);
            if (analysis == null)
            {
                return NotFound();
            }

            return View(analysis);
        }

        public async Task<IActionResult> SurveySummary()
        {
            var repeatedQuestions = await _analysisService.GetRepeatedQuestionsAsync();
            var surveyCompletions = await _analysisService.GetSurveyCompletionsAsync();
            var emailSends = await _analysisService.GetEmailSendsAsync();

            var surveyResponseAnalyses = new List<SurveyResponseAnalysis>();
            foreach (var completion in surveyCompletions)
            {
                var analysis = await _analysisService.GetSurveyResponseAnalysisAsync(completion.EntityId);
                if (analysis != null)
                {
                    surveyResponseAnalyses.Add(analysis);
                }
            }

            var summary = new SurveySummaryViewModel
            {
                RepeatedQuestions = repeatedQuestions,
                SurveyCompletions = surveyCompletions,
                EmailSends = emailSends,
                SurveyResponseAnalyses = surveyResponseAnalyses
            };

            return View(summary);
        }
        public async Task<IActionResult> QuestionTrendAnalysis(int id)
        {
            var trendAnalysis = await _analysisService.GetQuestionTrendAnalysisAsync(id);
            return View(trendAnalysis);
        }
    }
}
