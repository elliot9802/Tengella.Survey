using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

        [HttpGet]
        public IActionResult GetDeleteViewModel(string entityType, int entityId)
        {
            DeleteViewModel model = new DeleteViewModel();
            switch (entityType)
            {
                case "Recipient":
                    var recipient = _context.Recipients.Find(entityId);
                    if (recipient == null) return NotFound();
                    model = new DeleteViewModel
                    {
                        Title = "Delete Recipient",
                        DeleteAction = nameof(DeleteRecipientsConfirmed),
                        ReturnController = "EmailManagement",
                        MultipleProperties = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "Email", recipient.Email },
                        { "Name", recipient.Name }
                    }
                },
                        PropertyIcons = new Dictionary<string, string>
                {
                    { "Email", "fas fa-envelope" },
                    { "Name", "fas fa-user" }
                }
                    };
                    break;

                case "EmailTemplate":
                    var template = _context.EmailTemplates.Find(entityId);
                    if (template == null) return NotFound();
                    model = new DeleteViewModel
                    {
                        Title = "Delete Email Template",
                        DeleteAction = nameof(DeleteEmailTemplateConfirmed),
                        ReturnController = "EmailManagement",
                        MultipleProperties = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "Name", template.Name },
                        { "Subject", template.Subject }
                    }
                },
                        PropertyIcons = new Dictionary<string, string>
                {
                    { "Name", "fas fa-file-alt" },
                    { "Subject", "fas fa-envelope-open-text" }
                }
                    };
                    break;
            }
            return PartialView("_DeleteConfirmationModal", model);
        }

        public async Task<IActionResult> DeleteRecipients(List<int> ids)
        {
            var recipients = await _context.Recipients.Where(r => ids.Contains(r.RecipientId)).ToListAsync();
            if (recipients.Count == 0)
            {
                return NotFound();
            }

            var model = new DeleteViewModel
            {
                Title = "Delete Recipients",
                DeleteAction = nameof(DeleteRecipientsConfirmed),
                ReturnController = "EmailManagement",
                MultipleProperties = recipients.ConvertAll(r => new Dictionary<string, string>
                {
                    { "Email", r.Email },
                    { "Name", r.Name }
                }),
                PropertyIcons = new Dictionary<string, string>
                {
                    { "Email", "fas fa-envelope" },
                    { "Name", "fas fa-user" }
                }
            };

            return View("Delete", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRecipientsConfirmed(List<int> RecipientId)
        {
            var recipients = await _context.Recipients.Where(r => RecipientId.Contains(r.RecipientId)).ToListAsync();
            if (recipients.Count != 0)
            {
                _context.Recipients.RemoveRange(recipients);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { activeTab = "manage-recipients" });
        }

        public IActionResult UploadRecipients()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadRecipients(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Please select a valid file.");
                return View();
            }

            var recipients = new List<Recipient>();

            await using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets.First();
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var recipient = new Recipient
                    {
                        Email = worksheet.Cells[row, 1].Value?.ToString() ?? string.Empty,
                        Name = worksheet.Cells[row, 2].Value?.ToString() ?? string.Empty,
                        OptedOut = false
                    };
                    recipients.Add(recipient);
                }
            }

            _context.Recipients.AddRange(recipients);
            await _context.SaveChangesAsync();

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

        public async Task<IActionResult> DeleteEmailTemplates(List<int> ids)
        {
            var emailTemplates = await _context.EmailTemplates.Where(et => ids.Contains(et.EmailTemplateId)).ToListAsync();
            if (emailTemplates.Count == 0)
            {
                return NotFound();
            }

            var model = new DeleteViewModel
            {
                Title = "Delete Email Template",
                DeleteAction = nameof(DeleteEmailTemplateConfirmed),
                ReturnController = "EmailManagement",
                MultipleProperties = emailTemplates.ConvertAll(et => new Dictionary<string, string>
                {
                    { "EmailTemplateId", et.EmailTemplateId.ToString() },
                    { "Subject", et.Subject },
                    { "Name", et.Name }
                })
            };

            return View("Delete", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmailTemplateConfirmed(List<int> EmailTemplateId)
        {
            var emailTemplate = await _context.EmailTemplates.Where(et => EmailTemplateId.Contains(et.EmailTemplateId)).ToListAsync();
            if (emailTemplate.Count != 0)
            {
                _context.EmailTemplates.RemoveRange(emailTemplate);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { activeTab = "manage-templates" });
        }

        [HttpGet]
        public async Task<IActionResult> OptOut(string email)
        {
            var recipient = await _context.Recipients.FirstOrDefaultAsync(r => r.Email == email);
            if (recipient == null)
            {
                return NotFound();
            }
            recipient.OptedOut = true;
            await _context.SaveChangesAsync();
            return Content("You have successfully opted out.");
        }
    }
}