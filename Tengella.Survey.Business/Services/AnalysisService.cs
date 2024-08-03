using Microsoft.EntityFrameworkCore;
using Tengella.Survey.Business.DTOs;
using Tengella.Survey.Business.Interfaces;
using Tengella.Survey.Data;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.Business.Services
{
    public class AnalysisService(SurveyDbContext context) : IAnalysisService
    {
        private readonly SurveyDbContext _context = context;

        public async Task LogEntityCountChangeAsync<T>(string entityName, string logType, int countChange, int? entityId = null) where T : class
        {
            var log = await _context.AnalysisLogs
                .FirstOrDefaultAsync(l => l.EntityName == entityName && l.LogType == logType);

            if (log == null)
            {
                if (countChange > 0)
                {
                    log = new AnalysisLog
                    {
                        LogType = logType,
                        EntityId = entityId ?? 0,
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

        public async Task LogEntityCountsAsync<T>(IEnumerable<T> entities, string logType, Func<T, Task<bool>> predicate) where T : class
        {
            foreach (var entity in entities)
            {
                if (await predicate(entity))
                {
                    var entityName = entity.GetType().GetProperty("Text")?.GetValue(entity)?.ToString();
                    var entityId = (int?)entity.GetType().GetProperty("QuestionId")?.GetValue(entity);
                    await LogEntityCountChangeAsync<T>(entityName, logType, 1, entityId);
                }
            }
        }

        public async Task LogSurveyResponseAsync(int surveyFormId, int responseCount)
        {
            var survey = await _context.SurveyForms.FindAsync(surveyFormId);
            if (survey != null)
            {
                await LogEntityCountChangeAsync<SurveyForm>(survey.Name, "SurveyResponse", responseCount, surveyFormId);
            }
        }

        public async Task<bool> IsQuestionRepeatedAsync(string questionText, int excludedQuestionId)
        {
            return await _context.Questions.AnyAsync(q => q.Text == questionText && q.QuestionId != excludedQuestionId);
        }

        public async Task<bool> HasQuestionBeenRepeatedBeforeAsync(string questionText)
        {
            return await _context.AnalysisLogs.AnyAsync(l => l.EntityName == questionText && l.LogType == "RepeatedQuestion");
        }

        public async Task<List<AnalysisLog>> GetLogsByTypeAsync(string logType)
        {
            return await _context.AnalysisLogs
                .Where(log => log.LogType == logType)
                .OrderByDescending(log => log.Count)
                .ToListAsync();
        }

        public async Task<SurveyResponseAnalysis> GetSurveyResponseAnalysisAsync(int surveyFormId)
        {
            var survey = await _context.SurveyForms
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Responses)
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(s => s.SurveyFormId == surveyFormId);

            if (survey == null) return null;

            var emailSentLog = await _context.AnalysisLogs
                .Where(log => log.EntityId == surveyFormId && log.LogType == "SurveyEmailSent")
                .SumAsync(log => log.Count);

            var responseLog = await _context.AnalysisLogs
                .Where(log => log.EntityId == surveyFormId && log.LogType == "SurveyResponse")
                .SumAsync(log => log.Count);

            var totalResponses = survey.Questions.Sum(q => q.Responses.Count);
            var totalRespondents = survey.Questions.SelectMany(q => q.Responses).Select(r => r.ResponseGroupId).Distinct().Count();

            var responseRate = emailSentLog > 0 ? (double)responseLog / emailSentLog : 0.0;

            var questionResponseCounts = survey.Questions.ToDictionary(
                q => q.Text,
                q => q.Responses.Count
            );

            var optionResponseRates = survey.Questions
                .Where(q => q.Type == "Radio")
                .SelectMany(q => q.Responses, (q, r) => new { QuestionText = q.Text, r.OptionId, r })
                .GroupBy(qr => new { qr.QuestionText, qr.OptionId })
                .ToDictionary(
                    g => g.Key.QuestionText + "|" + _context.Options.First(o => o.OptionId == g.Key.OptionId).Text,
                    g =>
                    {
                        var questionTotalResponses = survey.Questions.First(q => q.Text == g.Key.QuestionText).Responses.Count;
                        var responseCount = g.Count();
                        var responseRate = (double)responseCount / questionTotalResponses;
                        return new { Count = responseCount, Rate = responseRate };
                    }
                );

            var shortAnswerResponses = survey.Questions
                .Where(q => q.Type == "Open")
                .ToDictionary(
                    q => q.Text,
                    q => q.Responses.Select(r => r.TextResponse).ToList()
                );

            return new SurveyResponseAnalysis
            {
                SurveyFormId = surveyFormId,
                TotalResponses = totalResponses,
                LastResponseDate = survey.Questions.SelectMany(q => q.Responses).Max(r => (DateTime?)r.ResponseDate),
                ResponseRate = responseRate,
                QuestionResponseCounts = questionResponseCounts,
                OptionResponseRates = optionResponseRates.ToDictionary(k => k.Key, v => v.Value.Rate),
                OptionResponseCounts = optionResponseRates.ToDictionary(k => k.Key, v => v.Value.Count),
                ShortAnswerResponses = shortAnswerResponses
            };
        }

        public async Task<List<AnalysisLog>> GetRepeatedQuestionsAsync()
        {
            return await _context.AnalysisLogs
                .Where(l => l.LogType == "RepeatedQuestion")
                .OrderByDescending(l => l.Count)
                .ToListAsync();
        }

        public async Task<List<AnalysisLog>> GetSurveyCompletionsAsync()
        {
            return await _context.AnalysisLogs
                .Where(l => l.LogType == "SurveyResponse")
                .OrderByDescending(l => l.Count)
                .ToListAsync();
        }

        public async Task<List<AnalysisLog>> GetEmailSendsAsync()
        {
            return await _context.AnalysisLogs
                .Where(l => l.LogType == "SurveyEmailSent")
                .OrderByDescending(l => l.Count)
                .ToListAsync();
        }
    }
}