using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using Tengella.Survey.Business.DTOs;
using Tengella.Survey.Business.Interfaces;
using Tengella.Survey.Data;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.Business.Services
{
    public class EmailService(SurveyDbContext context, IAnalysisService analysisService) : IEmailService
    {
        private readonly SurveyDbContext _context = context;
        private readonly IAnalysisService _analysisService = analysisService;

        public async Task<List<EmailTemplate>> GetEmailTemplatesAsync()
        {
            return await _context.EmailTemplates.ToListAsync();
        }

        public async Task<List<Recipient>> GetRecipientsAsync()
        {
            return await _context.Recipients.ToListAsync();
        }
        public async Task<List<Recipient>> GetRecipientsByIdsAsync(List<int> recipientIds)
        {
            return await _context.Recipients.Where(r => recipientIds.Contains(r.RecipientId) && !r.OptedOut).ToListAsync();
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpClient = new SmtpClient("smtp-mail.outlook.com")
            {
                UseDefaultCredentials = false,
                Port = 587,
                Credentials = new NetworkCredential("email@hotmail.com", "password"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("email@hotmail.com"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task<bool> SendSurveyEmailAsync(int templateId, Recipient recipient, string surveyLink, string optOutLink)
        {
            var template = await _context.EmailTemplates.FindAsync(templateId) ?? throw new InvalidOperationException("Template not found");

            var emailBody = template.Body
                .Replace("{RecipientName}", recipient.Name)
                .Replace("{SurveyLink}", surveyLink)
                .Replace("{OptOutLink}", optOutLink);

            try
            {
                await SendEmailAsync(recipient.Email, template.Subject, emailBody);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<EmailSendResult> SendSurveyEmailsAsync(int templateId, List<int> recipientIds, int surveyFormId, string baseUrl)
        {
            var surveyLink = $"{baseUrl}/Response/RespondSurvey/{surveyFormId}";
            var recipients = await GetRecipientsByIdsAsync(recipientIds);
            var failedRecipients = new List<string>();

            foreach (var recipient in recipients)
            {
                var optOutLink = $"{baseUrl}/EmailManagement/OptOut?email={recipient.Email}";

                var emailSent = await SendSurveyEmailAsync(templateId, recipient, surveyLink, optOutLink);
                if (!emailSent)
                {
                    failedRecipients.Add(recipient.Email);
                }
            }

            var survey = await _context.SurveyForms.FindAsync(surveyFormId);
            if (survey != null)
            {
                await _analysisService.LogEntityCountChangeAsync<SurveyForm>(survey.Name, "SurveyEmailSent", recipients.Count, surveyFormId);
            }

            return new EmailSendResult
            {
                Success = failedRecipients.Count == 0,
                FailedRecipients = failedRecipients,
                ErrorMessage = failedRecipients.Count > 0 ? "Visa e-postmeddelanden gick inte att skicka." : string.Empty
            };
            }
    }
}