namespace Tengella.Survey.Business.DTOs
{
    public class SurveyResponseAnalysis
    {
        public int SurveyFormId { get; set; }
        public int TotalResponses { get; set; }
        public DateTime? LastResponseDate { get; set; }
        public double ResponseRate { get; set; }
        public Dictionary<string, int> QuestionResponseCounts { get; set; }
        public Dictionary<string, double> OptionResponseRates { get; set; }
        public Dictionary<string, int> OptionResponseCounts { get; set; }
        public Dictionary<string, List<string>> ShortAnswerResponses { get; set; }
    }
}
