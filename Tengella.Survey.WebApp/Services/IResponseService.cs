using Tengella.Survey.Data.Models;

namespace Tengella.Survey.WebApp.Services
{
    public interface IResponseService
    {
        Task<IEnumerable<Response>> CreateResponsesAsync(SurveyForm survey, IFormCollection form);
        Task SaveResponsesAsync(IEnumerable<Response> responses);
    }
}
