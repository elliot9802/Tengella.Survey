using Microsoft.AspNetCore.Http;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.Business.Interfaces
{
    public interface IResponseService
    {
        Task<IEnumerable<Response>> CreateResponsesAsync(SurveyForm survey, IFormCollection form);
        Task SaveResponsesAsync(IEnumerable<Response> responses);
    }
}
