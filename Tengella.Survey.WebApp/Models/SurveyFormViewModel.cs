using System.ComponentModel.DataAnnotations;

namespace Tengella.Survey.WebApp.Models
{
    public class SurveyFormViewModel
    {
        public int SurveyFormId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        public DateTime ClosingDate { get; set; }

        public bool IsClosed { get; set; }

        public ICollection<QuestionViewModel> Questions { get; set; } = [];
    }
}
