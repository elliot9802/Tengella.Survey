namespace Tengella.Survey.Business.DTOs
{
    public class SurveySummaryItem
    {
        public int SurveyFormId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime ClosingDate { get; set; }
        public int TotalResponses { get; set; }
        public int TotalTimesTaken { get; set; }
    }
}
