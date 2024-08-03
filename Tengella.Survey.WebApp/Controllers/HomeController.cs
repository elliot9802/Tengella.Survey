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

        [HttpGet]
        public async Task<IActionResult> CheckSurvey(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                TempData["ErrorMessage"] = "The survey does not exist.";
                return RedirectToAction("Index");
            }

            if (survey.ClosingDate < DateTime.Today)
            {
                TempData["ErrorMessage"] = "This survey is closed and cannot be responded to.";
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