namespace Tengella.Survey.Data.Models
{
    public class AnalysisLog
    {
        public int AnalysisLogId { get; set; }
        public string LogType { get; set; } // e.g., "RepeatedQuestion", "SurveyCompletion", "EmailSent"
        public int EntityId { get; set; } // e.g., QuestionId, SurveyFormId
        public string EntityName { get; set; } // e.g., Question text, Survey name
        public int Count { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
