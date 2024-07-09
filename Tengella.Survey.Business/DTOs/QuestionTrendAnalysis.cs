namespace Tengella.Survey.Business.DTOs
{
    public class QuestionTrendAnalysis
    {
        public int QuestionId { get; set; }
        public List<QuestionTrend> Trends { get; set; } = [];
    }
}
