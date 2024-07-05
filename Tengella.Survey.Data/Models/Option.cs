using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Tengella.Survey.Data.Models
{
    public class Option
    {
        public int OptionId { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;
        public int QuestionId { get; set; }

        [ValidateNever]
        public Question Question { get; set; }
    }
}
