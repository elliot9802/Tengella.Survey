namespace Tengella.Survey.Data.Models
{
    public class OptionAnalysis
    {
        public int OptionAnalysisId { get; set; }
        public int OptionId { get; set; }
        public int QuestionAnalysisId { get; set; }
        public int ResponseCount { get; set; }
        public Option Option { get; set; }
        public QuestionAnalysis QuestionAnalysis { get; set; }
    }
}
