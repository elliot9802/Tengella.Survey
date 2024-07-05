using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Tengella.Survey.Data.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public int SurveyFormId { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        [ValidateNever]
        public SurveyForm SurveyForm { get; set; }

        [Required]
        public ICollection<Option> Options { get; set; } = [];
        public ICollection<Response> Responses { get; set; } = [];
    }
}
