namespace Tengella.Survey.Business.DTOs
{
    public class EmailSendResult
    {
        public bool Success { get; set; }
        public List<string> FailedRecipients { get; set; } = [];
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
