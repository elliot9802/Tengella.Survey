using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Tengella.Survey.WebApp.Services;
using WebApp.Models;

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


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}