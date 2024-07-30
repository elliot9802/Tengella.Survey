using Tengella.Survey.Business.DTOs;
using Tengella.Survey.Business.DTOs.Analysis;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.Business.Interfaces
{
    public interface IAnalysisService
    {
        Task<List<AnalysisLog>> GetLogsByTypeAsync(string logType);
        Task<SurveyAnalysis> AnalyzeSurveyAsync(int surveyFormId);
        Task<QuestionTrendAnalysis> GetQuestionTrendAnalysisAsync(int questionId);

        Task<SurveySummary> GetSurveySummaryAsync();

        Task<Dictionary<string, int>> GetRepeatedQuestionsAsync();

    }
}
