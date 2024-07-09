using System.ComponentModel.DataAnnotations;

namespace Tengella.Survey.Data.Models
{
    public class Recipient
    {
        public int RecipientId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;
        public bool OptedOut { get; set; }
    }
}
