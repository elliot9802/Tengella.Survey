namespace Tengella.Survey.Business.DTOs
{
    public class RepeatedQuestionAnalysis
    {
        public string QuestionText { get; set; }
        public List<OptionResponseCount> OptionResponseCounts { get; set; }
        public List<string> OpenResponses { get; set; }
    }
}