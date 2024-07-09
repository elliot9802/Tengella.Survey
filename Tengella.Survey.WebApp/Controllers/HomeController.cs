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


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}