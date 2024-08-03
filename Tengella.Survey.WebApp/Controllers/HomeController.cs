using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tengella.Survey.Business.Interfaces;
using Tengella.Survey.WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController(ISurveyService surveyService) : Controller
    {
        private readonly ISurveyService _surveyService = surveyService;

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            var surveys = await _surveyService.GetSurveysAsync();
            return View(surveys);
        }

        // GET: Home/CheckSurvey/5
        [HttpGet]
        public async Task<IActionResult> CheckSurvey(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                TempData["ErrorMessage"] = "Enkäten finns inte.";
                return RedirectToAction("Index");
            }

            if (survey.ClosingDate < DateTime.Today)
            {
                TempData["ErrorMessage"] = "Denna enkät är stängd och kan inte besvaras.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("RespondSurvey", "Response", new { id });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}