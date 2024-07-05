using Tengella.Survey.Data.Models;
using static Tengella.Survey.WebApp.Services.AnalysisService;

namespace Tengella.Survey.WebApp.Services
{
    public interface IAnalysisService
    {
        Task<SurveyAnalysis> AnalyzeSurveyAsync(int surveyFormId);
        Task<QuestionTrendAnalysis> GetQuestionTrendAnalysisAsync(int questionId);

        Task<SurveySummary> GetSurveySummaryAsync();

    }
}
