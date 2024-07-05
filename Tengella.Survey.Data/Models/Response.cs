using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Tengella.Survey.Data.Models
{
    public class Response
    {
        public int ResponseId { get; set; }
        public Guid ResponseGroupId { get; set; }
        public int SurveyFormId { get; set; }
        public int QuestionId { get; set; }
        public int? OptionId { get; set; }  // Nullable for open-ended responses
        public string? TextResponse { get; set; }  // Nullable for option responses
        public DateTime ResponseDate { get; set; } = DateTime.Today;

        [ValidateNever]
        public SurveyForm SurveyForm { get; set; }

        [ValidateNever]
        public Question Question { get; set; }

        [ValidateNever]
        public Option? Option { get; set; }
    }
}
