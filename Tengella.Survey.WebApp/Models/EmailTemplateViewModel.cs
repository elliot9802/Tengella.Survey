namespace Tengella.Survey.WebApp.Models
{
    public class EmailTemplateViewModel
    {
        public int EmailTemplateId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
