using System.ComponentModel.DataAnnotations;
using Tengella.Survey.Data.Models;
using Tengella.Survey.WebApp.Validation;

namespace Tengella.Survey.WebApp.Models
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public int SurveyFormId { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        [OptionsCountValidation]
        public ICollection<Option> Options { get; set; } = new List<Option>();
    }
}
