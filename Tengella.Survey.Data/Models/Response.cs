using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Tengella.Survey.Data.Models
{
    public class Response
    {
        public int ResponseId { get; set; }
        public int SurveyFormId { get; set; }
        [ValidateNever]
        public SurveyForm SurveyForm { get; set; }
        public string Answer { get; set; } = string.Empty;
    }
}
