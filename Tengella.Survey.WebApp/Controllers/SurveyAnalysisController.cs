using Microsoft.AspNetCore.Mvc;
using Tengella.Survey.Business.DTOs;
using Tengella.Survey.Business.Interfaces;
using Tengella.Survey.WebApp.Models;

namespace Tengella.Survey.WebApp.Controllers
{
    public class SurveyAnalysisController(IAnalysisService analysisService) : Controller
    {
        private readonly IAnalysisService _analysisService = analysisService;

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

            var surveySummaryData = surveyResponseAnalyses.Select(analysis => new SurveySummaryData
            {
                SurveyFormId = analysis.SurveyFormId,
                SurveyName = analysis.SurveyName,
                TotalResponses = analysis.TotalResponses,
                ResponseRate = analysis.ResponseRate,
                LastResponseDate = analysis.LastResponseDate,
                EmailSends = emailSends.FirstOrDefault(e => e.EntityId == analysis.SurveyFormId)?.Count ?? 0,
                SurveyCompletions = surveyCompletions.FirstOrDefault(c => c.EntityId == analysis.SurveyFormId)?.Count ?? 0
            }).ToList();

            var repeatedQuestionAnalyses = new Dictionary<string, RepeatedQuestionAnalysis>();
            foreach (var repeatedQuestion in repeatedQuestions)
            {
                var analysis = await _analysisService.GetRepeatedQuestionAnalysisAsync(repeatedQuestion.EntityName);
                if (analysis != null)
                {
                    repeatedQuestionAnalyses.Add(repeatedQuestion.EntityName, analysis);
                }
            }

            var summary = new SurveySummaryViewModel
            {
                RepeatedQuestions = repeatedQuestions,
                SurveySummaryData = surveySummaryData,
                SurveyResponseAnalyses = surveyResponseAnalyses,
                RepeatedQuestionAnalyses = repeatedQuestionAnalyses
            };

            return View(summary);
        }
    }
}