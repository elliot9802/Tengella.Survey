namespace Tengella.Survey.WebApp.Models
{
    public class SendEmailViewModel
    {
        public int TemplateId { get; set; }
        public List<int> RecipientIds { get; set; } = [];
        public int SurveyFormId { get; set; }
    }
}
