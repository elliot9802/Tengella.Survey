using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Tengella.Survey.Business.Interfaces;
using Tengella.Survey.Data;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.Business.Services
{
    public class ResponseService(SurveyDbContext context, IAnalysisService analysisService, IHttpContextAccessor httpContext) : IResponseService
    {
        private readonly SurveyDbContext _context = context;
        private readonly IAnalysisService _analysisService = analysisService;
        private readonly IHttpContextAccessor _httpContext = httpContext;

        public Task<IEnumerable<Response>> CreateResponsesAsync(SurveyForm survey, IFormCollection form)
        {
            var responses = new List<Response>();
            var responseGroupId = Guid.NewGuid();
            var today = DateTime.Today;

            if (survey.ClosingDate < today)
            {
                return Task.FromResult(Enumerable.Empty<Response>());
            }

            foreach (var question in survey.Questions)
            {
                var questionIndex = survey.Questions.ToList().IndexOf(question);
                if (question.Type == "Radio")
                {
                    var selectedOption = form[$"Questions[{questionIndex}].Options"];
                    if (!string.IsNullOrEmpty(selectedOption) && int.TryParse(selectedOption, out int optionId))
                    {
                        var response = new Response
                        {
                            SurveyFormId = survey.SurveyFormId,
                            QuestionId = question.QuestionId,
                            OptionId = optionId,
                            ResponseDate = today,
                            ResponseGroupId = responseGroupId
                        };
                        responses.Add(response);
                    }
                }
                else if (question.Type == "Open")
                {
                    var textResponse = form[$"Questions[{questionIndex}].Response"];
                    if (!string.IsNullOrEmpty(textResponse))
                    {
                        var response = new Response
                        {
                            SurveyFormId = survey.SurveyFormId,
                            QuestionId = question.QuestionId,
                            TextResponse = textResponse,
                            ResponseDate = today,
                            ResponseGroupId = responseGroupId
                        };
                        responses.Add(response);
                    }
                }
            }

            var sessionKey = $"Survey_{survey.SurveyFormId}_Responses";
            var sessionResponses = JsonConvert.SerializeObject(responses);
            _httpContext.HttpContext.Session.SetString(sessionKey, sessionResponses);

            return Task.FromResult((IEnumerable<Response>)responses);
        }

        public IEnumerable<Response> GetCachedResponses(int surveyFormId)
        {
            var sessionKey = $"Survey_{surveyFormId}_Responses";
            var sessionData = _httpContext.HttpContext.Session.GetString(sessionKey);
            if (sessionData != null)
            {
                return JsonConvert.DeserializeObject<List<Response>>(sessionData);
            }
            return [];
        }

        public async Task SaveResponsesAsync(IEnumerable<Response> responses)
        {
            _context.Responses.AddRange(responses);
            await _context.SaveChangesAsync();

            var surveyFormId = responses.First().SurveyFormId;
            await _analysisService.LogSurveyResponseAsync(surveyFormId, 1);
        }
    }
}
