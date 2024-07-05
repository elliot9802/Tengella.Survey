namespace Tengella.Survey.Data.Models
{
    public class SurveyAnalysis
    {
        public int SurveyAnalysisId { get; set; }
        public int SurveyFormId { get; set; }
        public DateTime AnalysisDate { get; set; } = DateTime.Today;
        public int TotalResponses { get; set; }
        public ICollection<QuestionAnalysis> QuestionAnalyses { get; set; } = [];

        public SurveyForm SurveyForm { get; set; }
    }
}
