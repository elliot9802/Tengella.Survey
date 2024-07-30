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

        [Required]
        public string Type { get; set; } = string.Empty; // Privatperson, Företag, Offentlig verksamhet

        public string? PersonNmr { get; set; }
        public string? OrgNmr { get; set; }
        public string? CustomerNmr { get; set; }
        public string? EmployeeNmr { get; set; }
        public bool OptedOut { get; set; }
    }
}
