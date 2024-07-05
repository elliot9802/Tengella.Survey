using System.ComponentModel.DataAnnotations;

namespace Tengella.Survey.Data.Models
{
    public class DistributionList
    {
        public int DistributionListId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty;
        public string? OrgNmr { get; set; }
        public string? PersonNmr { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
