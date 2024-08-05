using Tengella.Survey.Business.DTOs;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.Business.Interfaces
{
    public interface IAnalysisService
    {
        Task LogEntityCountChangeAsync<T>(string entityName, string logType, int countChange, int? entityId = null) where T : class;

        Task LogEntityCountsAsync<T>(IEnumerable<T> entities, string logType, Func<T, Task<bool>> predicate) where T : class;

        Task<bool> IsQuestionRepeatedAsync(string questionText, int excludedQuestionId);

        Task<bool> HasQuestionBeenRepeatedBeforeAsync(string questionText);

        Task LogSurveyResponseAsync(int surveyFormId, int responseCount);

        Task<SurveyResponseAnalysis> GetSurveyResponseAnalysisAsync(int surveyFormId);

        Task<List<AnalysisLog>> GetLogsByTypeAsync(string logType);

        Task<List<AnalysisLog>> GetEmailSendsAsync();

        Task<List<AnalysisLog>> GetSurveyCompletionsAsync();

        Task<List<AnalysisLog>> GetRepeatedQuestionsAsync();

        Task<RepeatedQuestionAnalysis> GetRepeatedQuestionAnalysisAsync(string questionText);
    }
}