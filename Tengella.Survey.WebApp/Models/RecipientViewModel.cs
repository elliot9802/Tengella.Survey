namespace Tengella.Survey.WebApp.Models
{
    public class RecipientViewModel
    {
        public int RecipientId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Privatperson, Företag, Offentlig verksamhet
        public string? PersonNmr { get; set; }
        public string? OrgNmr { get; set; }
        public string? CustomerNmr { get; set; }
        public string? EmployeeNmr { get; set; }
        public bool OptedOut { get; set; }
    }

}
