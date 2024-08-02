using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tengella.Survey.Business.Interfaces;
using Tengella.Survey.Data;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.Business.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly SurveyDbContext _context;
        private readonly ILogger<SurveyService> _logger;
        private readonly IAnalysisService _analysisService;

        public SurveyService(SurveyDbContext context, ILogger<SurveyService> logger, IAnalysisService analysisService)
        {
            _context = context;
            _logger = logger;
            _analysisService = analysisService;
        }

        public async Task<IEnumerable<SurveyForm>> GetSurveysAsync()
        {
            return await _context.SurveyForms
                .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
                .OrderByDescending(s => s.ClosingDate)
                .ToListAsync();
        }

        public async Task<SurveyForm?> GetSurveyByIdAsync(int id)
        {
            return await _context.SurveyForms
                .Include(s => s.Questions)
                .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(s => s.SurveyFormId == id);
        }

        public async Task CreateSurveyAsync(SurveyForm survey)
        {
            _context.SurveyForms.Add(survey);
            await _context.SaveChangesAsync();
            await _analysisService.LogEntityCountsAsync(survey.Questions, "RepeatedQuestion", async q => await _analysisService.IsQuestionRepeatedAsync(q.Text, q.QuestionId));
        }

        public async Task UpdateSurveyAsync(SurveyForm survey, List<int> questionsToRemove, List<int> optionsToRemove)
        {
            _context.SurveyForms.Update(survey);

            var oldQuestions = await _context.Questions.AsNoTracking()
                    .Where(q => q.SurveyFormId == survey.SurveyFormId)
                    .ToListAsync();

            foreach (var question in survey.Questions)
            {
                if (question.QuestionId == 0)
                {
                    _context.Entry(question).State = EntityState.Added;
                }
                else
                {
                    _context.Entry(question).State = EntityState.Modified;
                }

                foreach (var option in question.Options)
                {
                    if (option.OptionId == 0)
                    {
                        _context.Entry(option).State = EntityState.Added;
                    }
                    else
                    {
                        _context.Entry(option).State = EntityState.Modified;
                    }
                }
            }

            foreach (var questionId in questionsToRemove)
            {
                var question = await _context.Questions.FindAsync(questionId);
                if (question != null)
                {
                    await _analysisService.LogEntityCountChangeAsync<Question>(question.Text, "RepeatedQuestion", -1);
                    _context.Questions.Remove(question);
                }
            }

            foreach (var optionId in optionsToRemove)
            {
                var option = await _context.Options.FindAsync(optionId);
                if (option != null)
                {
                    _context.Options.Remove(option);
                }
            }

            await _context.SaveChangesAsync();

            await LogRepeatedQuestionsAsync(survey, oldQuestions);
        }

        private async Task LogRepeatedQuestionsAsync(SurveyForm survey, List<Question> oldQuestions)
        {
            var oldQuestionDict = oldQuestions.ToDictionary(q => q.QuestionId);

            foreach (var question in survey.Questions)
            {
                if (oldQuestionDict.TryGetValue(question.QuestionId, out var oldQuestion))
                {
                    if (oldQuestion.Text != question.Text)
                    {
                        // Old question text check
                        if (await _analysisService.HasQuestionBeenRepeatedBeforeAsync(oldQuestion.Text))
                        {
                            await _analysisService.LogEntityCountChangeAsync<Question>(oldQuestion.Text, "RepeatedQuestion", -1);
                        }

                        // New question text check
                        if (await _analysisService.IsQuestionRepeatedAsync(question.Text, question.QuestionId))
                        {
                            await _analysisService.LogEntityCountChangeAsync<Question>(question.Text, "RepeatedQuestion", 1);
                        }
                    }
                }
                else
                {
                    // New question
                    if (await _analysisService.IsQuestionRepeatedAsync(question.Text, question.QuestionId))
                    {
                        await _analysisService.LogEntityCountChangeAsync<Question>(question.Text, "RepeatedQuestion", 1);
                    }
                }
            }
        }

        public async Task DeleteSurveyAsync(int id)
        {
            var survey = await _context.SurveyForms
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(s => s.SurveyFormId == id);

            if (survey != null)
            {
                foreach (var question in survey.Questions)
                {
                    await _analysisService.LogEntityCountChangeAsync<Question>(question.Text, "RepeatedQuestion", -1);
                }
                await _analysisService.LogEntityCountChangeAsync<SurveyForm>(survey.Name, "SurveyEmailSent", -1, survey.SurveyFormId);

                _context.SurveyForms.Remove(survey);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SurveyForm?> CopySurveyAsync(int id)
        {
            var survey = await GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return null;
            }
            var newSurvey = new SurveyForm
            {
                Name = survey.Name + " Copy",
                Type = survey.Type,
                ClosingDate = survey.ClosingDate,
                Questions = survey.Questions.Select(q => new Question
                {
                    Text = q.Text,
                    Type = q.Type,
                    Options = q.Options.Select(o => new Option { Text = o.Text }).ToList()
                }).ToList()
            };

            _context.SurveyForms.Add(newSurvey);
            await _context.SaveChangesAsync();
            await _analysisService.LogEntityCountsAsync(newSurvey.Questions, "RepeatedQuestion", async q => await _analysisService.IsQuestionRepeatedAsync(q.Text, q.QuestionId));
            return newSurvey;
        }
    }
}
