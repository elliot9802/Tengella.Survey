using Tengella.Survey.Data.Models;

namespace Tengella.Survey.Business.DTOs.Analysis
{
    public class QuestionAnalysis
    {
        public int QuestionAnalysisId { get; set; }
        public int QuestionId { get; set; }
        public int SurveyAnalysisId { get; set; }
        public int TotalResponses { get; set; }
        public double AverageRating { get; set; }
        public ICollection<OptionAnalysis> OptionAnalyses { get; set; } = [];
        public Question Question { get; set; }
        public SurveyAnalysis SurveyAnalysis { get; set; }
    }
}
