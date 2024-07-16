using Tengella.Survey.Data.Models;

namespace Tengella.Survey.Business.Interfaces
{
    public interface ISurveyService
    {
        Task<IEnumerable<SurveyForm>> GetSurveysAsync();
        Task<SurveyForm?> GetSurveyByIdAsync(int id);
        Task CreateSurveyAsync(SurveyForm survey);
        Task UpdateSurveyAsync(SurveyForm survey, List<int> questionsToRemove, List<int> optionsToRemove);
        Task DeleteSurveyAsync(int id);
        Task<SurveyForm?> CopySurveyAsync(int id);
        Task<int> GetTotalResponsesAsync(int surveyFormId);
    }
}
