namespace Tengella.Survey.Business.DTOs
{
    public class SurveySummary
    {
        public List<SurveySummaryItem> Surveys { get; set; } = [];
        public List<SurveySummaryItem> ClosedSurveys { get; set; } = [];
    }
}
