using Tengella.Survey.Business.DTOs;
using Tengella.Survey.Business.DTOs.Analysis;

namespace Tengella.Survey.Business.Interfaces
{
    public interface IAnalysisService
    {
        Task<SurveyAnalysis> AnalyzeSurveyAsync(int surveyFormId);
        Task<QuestionTrendAnalysis> GetQuestionTrendAnalysisAsync(int questionId);

        Task<SurveySummary> GetSurveySummaryAsync();

        Task<Dictionary<string, int>> GetRepeatedQuestionsAsync();

    }
}
