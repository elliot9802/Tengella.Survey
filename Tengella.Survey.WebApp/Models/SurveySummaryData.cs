namespace Tengella.Survey.WebApp.Models
{
    public class SurveySummaryData
    {
        public int SurveyFormId { get; set; }
        public string SurveyName { get; set; }
        public int TotalResponses { get; set; }
        public double ResponseRate { get; set; }
        public DateTime? LastResponseDate { get; set; }
        public int EmailSends { get; set; }
        public int SurveyCompletions { get; set; }
    }
}
