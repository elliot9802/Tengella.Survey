using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Tengella.Survey.Data;
using Tengella.Survey.Data.Models;
using Tengella.Survey.WebApp.Models;

namespace Tengella.Survey.WebApp.Controllers
{
    public class RecipientController(SurveyDbContext context) : Controller
    {
        private readonly SurveyDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            return View(await _context.Recipients.ToListAsync());
        }

        public IActionResult CreateRecipient()
        {
            return View();
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
            return View(recipient);
        }

        public async Task<IActionResult> EditRecipient(int id)
        {
            var recipient = await _context.Recipients.FindAsync(id);
            if (recipient == null)
            {
                return NotFound();
            }
            return View(recipient);
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
            return View(recipient);
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
                ReturnController = "Recipient"
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

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets.First();
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        var recipient = new Recipient
                        {
                            Email = worksheet.Cells[row, 1].Value?.ToString(),
                            Name = worksheet.Cells[row, 2].Value?.ToString(),
                            OptedOut = false
                        };
                        recipients.Add(recipient);
                    }
                }
            }

            _context.Recipients.AddRange(recipients);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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