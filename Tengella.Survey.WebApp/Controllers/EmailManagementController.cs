﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
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
            var distributionLists = await _context.DistributionLists.Include(dl => dl.Recipients).ToListAsync();

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
                Type = r.Type,
                PersonNmr = r.PersonNmr,
                OrgNmr = r.OrgNmr,
                CustomerNmr = r.CustomerNmr,
                EmployeeNmr = r.EmployeeNmr,
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

            ViewBag.DistributionLists = distributionLists.Select(dl => new SelectListItem { Value = dl.DistributionListId.ToString(), Text = dl.Name }).ToList();
            ViewBag.Surveys = surveys.Select(s => new SelectListItem { Value = s.SurveyFormId.ToString(), Text = s.Name }).ToList();

            return View(new SendEmailViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(SendEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
                var recipientIds = model.DistributionListId.HasValue
                    ? _context.DistributionLists.Include(dl => dl.Recipients).First(dl => dl.DistributionListId == model.DistributionListId.Value).Recipients.ConvertAll(r => r.RecipientId)
                    : model.RecipientIds;

                var result = await _emailService.SendSurveyEmailsAsync(model.TemplateId, recipientIds, model.SurveyFormId, baseUrl);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Alla e-postmeddelanden skickade!";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Misslyckades med att skicka e-postmeddelanden till följande mottagare: {string.Join(", ", result.FailedRecipients)}";
                }

                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Misslyckades med att skicka e-postmeddelanden. Vänligen dubbelkolla inmatning.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SaveDistributionList(string listName, List<int> recipientIds)
        {
            if (string.IsNullOrWhiteSpace(listName) || recipientIds?.Any() != true)
            {
                TempData["ErrorMessage"] = "Ogiltigt namn eller mottagare för distributionslistan.";
                return RedirectToAction(nameof(Index));
            }

            var recipients = await _context.Recipients.Where(r => recipientIds.Contains(r.RecipientId)).ToListAsync();

            var distributionList = new DistributionList
            {
                Name = listName,
                Recipients = recipients
            };

            _context.DistributionLists.Add(distributionList);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Distributionslistan sparades!";
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
                return RedirectToAction(nameof(Index), new { activeTab = "manage-recipients" });
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
                return RedirectToAction(nameof(Index), new { activeTab = "manage-recipients" });
            }
            return PartialView("_CreateRecipient", recipient);
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
                return RedirectToAction(nameof(Index), new { activeTab = "manage-templates" });
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
                return RedirectToAction(nameof(Index), new { activeTab = "manage-templates" });
            }
            return PartialView("_CreateEmailTemplate", emailTemplate);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRecipients(List<int> ids)
        {
            var recipients = await _context.Recipients.Where(r => ids.Contains(r.RecipientId)).ToListAsync();
            if (recipients.Count != 0)
            {
                _context.Recipients.RemoveRange(recipients);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { activeTab = "manage-recipients" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEmailTemplate(List<int> ids)
        {
            var emailTemplates = await _context.EmailTemplates.Where(et => ids.Contains(et.EmailTemplateId)).ToListAsync();
            if (emailTemplates.Count != 0)
            {
                _context.EmailTemplates.RemoveRange(emailTemplates);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { activeTab = "manage-templates" });
        }

        public async Task<IActionResult> RecipientDetails(int id)
        {
            var recipient = await _context.Recipients.FindAsync(id);
            if (recipient == null)
            {
                return NotFound();
            }

            var model = new RecipientViewModel
            {
                RecipientId = recipient.RecipientId,
                Email = recipient.Email,
                Name = recipient.Name,
                Type = recipient.Type,
                PersonNmr = recipient.PersonNmr,
                OrgNmr = recipient.OrgNmr,
                CustomerNmr = recipient.CustomerNmr,
                EmployeeNmr = recipient.EmployeeNmr,
                OptedOut = recipient.OptedOut
            };

            return PartialView("_RecipientDetails", model);
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
            return Content("Du är nu avregistrerad och kommer inte att få några mer mejl.");
        }

        [HttpPost]
        public async Task<IActionResult> UploadRecipients(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Vänligen välj en giltlig fil.";
                return RedirectToAction(nameof(Index), new { activeTab = "manage-recipients" });
            }

            var recipients = new List<Recipient>();
            var validationErrors = new List<string>();

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
                        Type = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty,
                        PersonNmr = worksheet.Cells[row, 4].Value?.ToString(),
                        OrgNmr = worksheet.Cells[row, 5].Value?.ToString(),
                        CustomerNmr = worksheet.Cells[row, 6].Value?.ToString(),
                        EmployeeNmr = worksheet.Cells[row, 7].Value?.ToString(),
                        OptedOut = false
                    };

                    var validationContext = new ValidationContext(recipient);
                    var validationResults = new List<ValidationResult>();

                    if (!Validator.TryValidateObject(recipient, validationContext, validationResults, true))
                    {
                        foreach (var validationResult in validationResults)
                        {
                            validationErrors.Add($"Row {row}: {validationResult.ErrorMessage}");
                        }
                    }
                    else
                    {
                        recipients.Add(recipient);
                    }
                }
            }

            if (validationErrors.Any())
            {
                TempData["ErrorMessage"] = "Följande fel uppstod vid importering av mottagare: " + string.Join(", ", validationErrors);
                return RedirectToAction(nameof(Index), new { activeTab = "manage-recipients" });
            }
            _context.Recipients.AddRange(recipients);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { activeTab = "manage-recipients" });
        }
    }
}