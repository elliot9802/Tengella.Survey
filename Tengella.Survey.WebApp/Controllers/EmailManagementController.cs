using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tengella.Survey.Business.Interfaces;
using Tengella.Survey.Data;
using Tengella.Survey.Data.Models;
using Tengella.Survey.WebApp.Models;

namespace Tengella.Survey.WebApp.Controllers
{
    public class EmailManagementController(SurveyDbContext context, IEmailService emailService, ISurveyService surveyService) : Controller
    {
        private readonly SurveyDbContext _context = context;
        private readonly IEmailService _emailService = emailService;
        private readonly ISurveyService _surveyService = surveyService;

        public async Task<IActionResult> Index()
        {
            var emailTemplates = await _emailService.GetEmailTemplatesAsync();
            var recipients = await _emailService.GetRecipientsAsync();
            var surveys = await _surveyService.GetSurveysAsync();

            ViewBag.EmailTemplates = emailTemplates.ConvertAll(et => new EmailTemplateViewModel
            {
                EmailTemplateId = et.EmailTemplateId,
                Name = et.Name,
                Subject = et.Subject
            });

            // All recipients for the card display
            ViewBag.AllRecipients = recipients.ConvertAll(r => new RecipientViewModel
            {
                RecipientId = r.RecipientId,
                Email = r.Email,
                Name = r.Name,
                OptedOut = r.OptedOut
            });

            // Filtered recipients for the send survey form
            ViewBag.Recipients = recipients
                .Where(r => !r.OptedOut)
                .Select(r => new RecipientViewModel
                {
                    RecipientId = r.RecipientId,
                    Email = r.Email,
                    Name = r.Name
                }).ToList();

            ViewBag.Surveys = surveys.Select(s => new SelectListItem { Value = s.SurveyFormId.ToString(), Text = s.Name }).ToList();

            return View(new SendEmailViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(SendEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                var result = await _emailService.SendSurveyEmailsAsync(model.TemplateId, model.RecipientIds, model.SurveyFormId, baseUrl);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "All emails sent successfully!";
                }
                else
                {
                    TempData["SuccessMessage"] = "Some emails sent successfully!";
                    TempData["ErrorMessage"] = $"Failed to send emails to the following recipients: {string.Join(", ", result.FailedRecipients)}";
                }

                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Failed to send emails. Please check your input.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CreateRecipient()
        {
            return PartialView("_CreateRecipient", new Recipient());
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipient(Recipient recipient)
        {
            if (ModelState.IsValid)
            {
                _context.Recipients.Add(recipient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_CreateRecipient", recipient);
        }

        public async Task<IActionResult> EditRecipient(int id)
        {
            var recipient = await _context.Recipients.FindAsync(id);
            if (recipient == null)
            {
                return NotFound();
            }
            return PartialView("_CreateRecipient", recipient);
        }

        [HttpPost]
        public async Task<IActionResult> EditRecipient(Recipient recipient)
        {
            if (ModelState.IsValid)
            {
                _context.Recipients.Update(recipient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_CreateRecipient", recipient);
        }

        public async Task<IActionResult> DeleteRecipient(int id)
        {
            var recipient = await _context.Recipients.FindAsync(id);
            if (recipient == null)
            {
                return NotFound();
            }

            var model = new DeleteViewModel
            {
                Title = "Delete Recipient",
                EntityName = recipient.Email,
                Properties = new Dictionary<string, string>
                {
                    { "RecipientId", recipient.RecipientId.ToString() },
                    { "Email", recipient.Email },
                    { "Name", recipient.Name }
                },
                DeleteAction = nameof(DeleteRecipientConfirmed),
                ReturnController = "EmailManagement"
            };

            return View("Delete", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRecipientConfirmed(int recipientId)
        {
            var recipient = await _context.Recipients.FindAsync(recipientId);
            if (recipient != null)
            {
                _context.Recipients.Remove(recipient);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult CreateEmailTemplate()
        {
            return PartialView("_CreateEmailTemplate", new EmailTemplate());
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmailTemplate(EmailTemplate emailTemplate)
        {
            if (ModelState.IsValid)
            {
                _context.EmailTemplates.Add(emailTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_CreateEmailTemplate", emailTemplate);
        }

        public async Task<IActionResult> EditEmailTemplate(int id)
        {
            var emailTemplate = await _context.EmailTemplates.FindAsync(id);
            if (emailTemplate == null)
            {
                return NotFound();
            }
            return PartialView("_CreateEmailTemplate", emailTemplate);
        }

        [HttpPost]
        public async Task<IActionResult> EditEmailTemplate(EmailTemplate emailTemplate)
        {
            if (ModelState.IsValid)
            {
                _context.EmailTemplates.Update(emailTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_CreateEmailTemplate", emailTemplate);
        }

        public async Task<IActionResult> DeleteEmailTemplate(int id)
        {
            var emailTemplate = await _context.EmailTemplates.FindAsync(id);
            if (emailTemplate == null)
            {
                return NotFound();
            }

            var model = new DeleteViewModel
            {
                Title = "Delete Email Template",
                EntityName = emailTemplate.Name,
                Properties = new Dictionary<string, string>
                {
                    { "EmailTemplateId", emailTemplate.EmailTemplateId.ToString() },
                    { "Name", emailTemplate.Name },
                    { "Subject", emailTemplate.Subject }
                },
                DeleteAction = nameof(DeleteEmailTemplateConfirmed),
                ReturnController = "EmailManagement"
            };

            return View("Delete", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmailTemplateConfirmed(int emailTemplateId)
        {
            var emailTemplate = await _context.EmailTemplates.FindAsync(emailTemplateId);
            if (emailTemplate != null)
            {
                _context.EmailTemplates.Remove(emailTemplate);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
