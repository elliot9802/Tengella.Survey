using Tengella.Survey.Business.DTOs;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.WebApp.Models
{
    public class SurveySummaryViewModel
    {
        public List<AnalysisLog> RepeatedQuestions { get; set; }
        public List<AnalysisLog> SurveyCompletions { get; set; }
        public List<AnalysisLog> EmailSends { get; set; }
        public List<SurveyResponseAnalysis> SurveyResponseAnalyses { get; set; }
    }
}
