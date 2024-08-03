using Tengella.Survey.Data.Models;
using Tengella.Survey.Business.DTOs;
namespace Tengella.Survey.Business.Interfaces
{
    public interface IEmailService
    {
        Task<List<EmailTemplate>> GetEmailTemplatesAsync();
        Task<List<Recipient>> GetRecipientsAsync();
        Task<List<Recipient>> GetRecipientsByIdsAsync(List<int> recipientIds);
        Task SendEmailAsync(string email, string subject, string message);
        Task<bool> SendSurveyEmailAsync(int templateId, Recipient recipient, string surveyLink, string optOutLink);
        Task<EmailSendResult> SendSurveyEmailsAsync(int templateId, List<int> recipientIds, int surveyFormId, string baseUrl);
    }
}
