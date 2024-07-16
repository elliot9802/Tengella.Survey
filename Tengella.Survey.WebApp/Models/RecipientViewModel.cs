namespace Tengella.Survey.WebApp.Models
{
    public class RecipientViewModel
    {
        public int RecipientId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool OptedOut { get; set; }
    }

}
