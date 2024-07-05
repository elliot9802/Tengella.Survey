using Microsoft.EntityFrameworkCore;
using Tengella.Survey.Data;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.WebApp.Services
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
        }

        public async Task DeleteSurveyAsync(int id)
        {
            var survey = await _context.SurveyForms.FindAsync(id);
            if (survey != null)
            {
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
            return newSurvey;
        }
    }
}
