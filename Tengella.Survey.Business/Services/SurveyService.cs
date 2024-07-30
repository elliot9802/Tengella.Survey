using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tengella.Survey.Business.Interfaces;
using Tengella.Survey.Data;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.Business.Services
{
    public class SurveyService(SurveyDbContext context, ILogger<SurveyService> logger) : ISurveyService
    {
        private readonly SurveyDbContext _context = context;
        private readonly ILogger<SurveyService> _logger = logger;

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
            await LogEntityCounts(survey.Questions, "RepeatedQuestion");
        }

        public async Task UpdateSurveyAsync(SurveyForm survey, List<int> questionsToRemove, List<int> optionsToRemove)
        {
            _context.SurveyForms.Update(survey);

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
                    await LogEntityCountChange(question, "RepeatedQuestion", -1);
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
            await LogEntityCounts(survey.Questions, "RepeatedQuestion");
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
                    await LogEntityCountChange(question, "RepeatedQuestion", -1);
                }
                await LogEntityCountChange(survey, "SurveyEmailSent", -1);

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
            await LogEntityCounts(newSurvey.Questions, "RepeatedQuestion");
            return newSurvey;
        }

        public async Task<int> GetTotalResponsesAsync(int surveyFormId)
        {
            var survey = await _context.SurveyForms
                .Include(s => s.Questions)
                .ThenInclude(q => q.Responses)
                .FirstOrDefaultAsync(s => s.SurveyFormId == surveyFormId);

            return survey == null
                ? throw new InvalidOperationException("Survey not found.")
                : survey.Questions
                .SelectMany(q => q.Responses)
                .GroupBy(r => r.ResponseGroupId)
                .Count();
        }

        private async Task LogEntityCounts<T>(IEnumerable<T> entities, string logType) where T : class
        {
            foreach (var entity in entities)
            {
                await LogEntityCountChange(entity, logType, 1);
            }
        }

        private async Task LogEntityCountChange<T>(T entity, string logType, int countChange) where T : class
        {
            if (entity == null) return;

            var entityName = entity.GetType().GetProperty("Text")?.GetValue(entity)?.ToString();
            var entityId = (int?)entity.GetType().GetProperty("QuestionId")?.GetValue(entity);

            if (string.IsNullOrEmpty(entityName) || entityId == null) return;

            var log = await _context.AnalysisLogs
                .FirstOrDefaultAsync(l => l.EntityName == entityName && l.LogType == logType);

            if (log == null)
            {
                if (countChange > 0)
                {
                    log = new AnalysisLog
                    {
                        LogType = logType,
                        EntityId = entityId.Value,
                        EntityName = entityName,
                        Count = countChange,
                        LastUpdated = DateTime.UtcNow
                    };
                    _context.AnalysisLogs.Add(log);
                }
            }
            else
            {
                log.Count += countChange;
                log.LastUpdated = DateTime.UtcNow;

                if (log.Count <= 0)
                {
                    _context.AnalysisLogs.Remove(log);
                }
                else
                {
                    _context.AnalysisLogs.Update(log);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
