using Tengella.Survey.Data;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.WebApp.Services
{
    public class ResponseService(SurveyDbContext context) : IResponseService
    {
        private readonly SurveyDbContext _context = context;
        public Task<IEnumerable<Response>> CreateResponsesAsync(SurveyForm survey, IFormCollection form)
        {
            var responses = new List<Response>();
            var responseGroupId = Guid.NewGuid();

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
                            ResponseDate = DateTime.Today,
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
                            ResponseDate = DateTime.Today,
                            ResponseGroupId = responseGroupId
                        };
                        responses.Add(response);
                    }
                }
            }

            return Task.FromResult((IEnumerable<Response>)responses);
        }

        public async Task SaveResponsesAsync(IEnumerable<Response> responses)
        {
            _context.Responses.AddRange(responses);
            await _context.SaveChangesAsync();
        }
    }
}
