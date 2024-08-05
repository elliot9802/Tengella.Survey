using Tengella.Survey.Business.DTOs;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.WebApp.Models
{
    public class SurveySummaryViewModel
    {
        public List<AnalysisLog> RepeatedQuestions { get; set; }
        public List<SurveySummaryData> SurveySummaryData { get; set; } = new List<SurveySummaryData>();
        public List<SurveyResponseAnalysis> SurveyResponseAnalyses { get; set; }
        public Dictionary<string, RepeatedQuestionAnalysis> RepeatedQuestionAnalyses { get; set; } = new Dictionary<string, RepeatedQuestionAnalysis>();
    }
}