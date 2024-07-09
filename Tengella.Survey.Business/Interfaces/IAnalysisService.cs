using Tengella.Survey.Business.DTOs;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.Business.Interfaces
{
    public interface IAnalysisService
    {
        Task<SurveyAnalysis> AnalyzeSurveyAsync(int surveyFormId);
        Task<QuestionTrendAnalysis> GetQuestionTrendAnalysisAsync(int questionId);

        Task<SurveySummary> GetSurveySummaryAsync();

    }
}
