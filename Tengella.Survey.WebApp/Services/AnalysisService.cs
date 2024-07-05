using Microsoft.EntityFrameworkCore;
using Tengella.Survey.Data;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.WebApp.Services
{
    public class AnalysisService(SurveyDbContext context) : IAnalysisService
    {
        private readonly SurveyDbContext _context = context;

        public async Task<SurveyAnalysis> AnalyzeSurveyAsync(int surveyFormId)
        {
            var survey = await _context.SurveyForms
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Options)
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Responses)
                .AsSplitQuery()
                .FirstOrDefaultAsync(s => s.SurveyFormId == surveyFormId);

            if (survey == null)
            {
                throw new InvalidOperationException("Survey not found.");
            }

            var surveyAnalysis = new SurveyAnalysis
            {
                SurveyFormId = surveyFormId,
                TotalResponses = survey.Questions.Sum(q => q.Responses.Count),
                SurveyForm = survey
            };

            foreach (var question in survey.Questions)
            {
                var questionAnalysis = new QuestionAnalysis
                {
                    QuestionId = question.QuestionId,
                    TotalResponses = question.Responses.Count,
                    Question = question
                };

                if (question.Type == "Radio")
                {
                    foreach (var option in question.Options)
                    {
                        var optionAnalysis = new OptionAnalysis
                        {
                            OptionId = option.OptionId,
                            ResponseCount = question.Responses.Count(r => r.OptionId == option.OptionId),
                            Option = option
                        };
                        questionAnalysis.OptionAnalyses.Add(optionAnalysis);
                    }
                    questionAnalysis.AverageRating = questionAnalysis.OptionAnalyses.Count != 0
                        ? questionAnalysis.OptionAnalyses.Average(o => o.ResponseCount)
                        : 0;
                }

                surveyAnalysis.QuestionAnalyses.Add(questionAnalysis);
            }

            return surveyAnalysis;
        }

        public async Task<SurveySummary> GetSurveySummaryAsync()
        {
            var surveys = await _context.SurveyForms
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Responses)
                .ToListAsync();

            var surveySummaries = surveys.ConvertAll(s => new SurveySummaryItem
            {
                SurveyFormId = s.SurveyFormId,
                Name = s.Name,
                Type = s.Type,
                ClosingDate = s.ClosingDate,
                TotalResponses = s.Questions.Sum(q => q.Responses.Count),
                TotalTimesTaken = s.Questions.SelectMany(q => q.Responses)
                                            .GroupBy(r => r.ResponseGroupId)
                                            .Distinct()
                                            .Count()
            });

            return new SurveySummary { Surveys = surveySummaries };
        }

        public async Task<QuestionTrendAnalysis> GetQuestionTrendAnalysisAsync(int questionId)
        {
            var questionsResponses = await _context.Questions
                .Include(q => q.SurveyForm)
                .Include(q => q.Responses)
                .Where(q => q.QuestionId == questionId)
                .ToListAsync();

            return new QuestionTrendAnalysis
            {
                QuestionId = questionId,
                Trends = questionsResponses
                    .GroupBy(q => q.SurveyForm.Name)
                    .Select(grp => new QuestionTrend
                    {
                        SurveyFormName = grp.Key,
                        TotalResponses = grp.SelectMany(q => q.Responses).Count(),
                        AverageRating = grp.SelectMany(q => q.Responses)
                            .Average(r => double.TryParse(r.TextResponse, out var val) ? val : 0)
                    }).ToList()
            };
        }
    }

    public class SurveySummary
    {
        public List<SurveySummaryItem> Surveys { get; set; } = new();
    }

    public class SurveySummaryItem
    {
        public int SurveyFormId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime ClosingDate { get; set; }
        public int TotalResponses { get; set; }
        public int TotalTimesTaken { get; set; }
    }

    public class QuestionTrendAnalysis
    {
        public int QuestionId { get; set; }
        public List<QuestionTrend> Trends { get; set; } = new();
    }

    public class QuestionTrend
    {
        public string SurveyFormName { get; set; }
        public int TotalResponses { get; set; }
        public double AverageRating { get; set; }
    }
}
