using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using Tengella.Survey.Data.Models;
using Tengella.Survey.WebApp.Models;
using Tengella.Survey.WebApp.Services;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class HomeController(ILogger<HomeController> logger, ISurveyService surveyService) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly ISurveyService _surveyService = surveyService;

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            var surveys = await _surveyService.GetSurveysAsync();
            var currentDate = DateTime.Now;
            foreach (var survey in surveys)
            {
                if (survey.ClosingDate < currentDate)
                {
                    survey.IsClosed = true;
                }
            }
            return View(surveys);
        }

        // GET: Home/CreateSurvey/5
        public async Task<IActionResult> CreateSurvey(int? id)
        {
            if (id == null)
            {
                return View(new SurveyFormViewModel());
            }

            var survey = await _surveyService.GetSurveyByIdAsync(id.Value);
            if (survey == null)
            {
                return NotFound();
            }

            var surveyViewModel = new SurveyFormViewModel
            {
                SurveyFormId = survey.SurveyFormId,
                Name = survey.Name,
                Type = survey.Type,
                ClosingDate = survey.ClosingDate,
                Questions = survey.Questions.Select(q => new QuestionViewModel
                {
                    QuestionId = q.QuestionId,
                    SurveyFormId = q.SurveyFormId,
                    Text = q.Text,
                    Type = q.Type,
                    Options = q.Options
                }).ToList()
            };

            return View(surveyViewModel);
        }

        // POST: Home/CreateSurvey
        [HttpPost]
        public async Task<IActionResult> CreateSurvey(SurveyFormViewModel survey, string? questionsToRemove, string? optionsToRemove, bool isPreview = false)
        {
            var questionsToRemoveList = string.IsNullOrEmpty(questionsToRemove) ? new List<int>() : questionsToRemove.Split(',').Select(int.Parse).ToList();
            var optionsToRemoveList = string.IsNullOrEmpty(optionsToRemove) ? new List<int>() : optionsToRemove.Split(',').Select(int.Parse).ToList();

            if (ModelState.IsValid)
            {
                if (isPreview)
                {
                    TempData["SurveyPreview"] = JsonConvert.SerializeObject(survey);
                    TempData["QuestionsToRemove"] = questionsToRemove;
                    TempData["OptionsToRemove"] = optionsToRemove;
                    return RedirectToAction("PreviewSurvey");
                }

                var surveyForm = new SurveyForm
                {
                    SurveyFormId = survey.SurveyFormId,
                    Name = survey.Name,
                    Type = survey.Type,
                    ClosingDate = survey.ClosingDate,
                    Questions = survey.Questions.Select(q => new Question
                    {
                        QuestionId = q.QuestionId,
                        SurveyFormId = q.SurveyFormId,
                        Text = q.Text,
                        Type = q.Type,
                        Options = q.Options.Select(o => new Option
                        {
                            OptionId = o.OptionId,
                            Text = o.Text,
                            QuestionId = o.QuestionId
                        }).ToList()
                    }).ToList()
                };

                if (survey.SurveyFormId == 0)
                {
                    await _surveyService.CreateSurveyAsync(surveyForm);
                }
                else
                {
                    await _surveyService.UpdateSurveyAsync(surveyForm, questionsToRemoveList, optionsToRemoveList);
                }

                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning("ModelState is not valid. Errors: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return View(survey);
        }

        // GET: Home/DeleteSurvey/5
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // POST: Home/DeleteSurvey/5
        [HttpPost, ActionName("DeleteSurveyConfirmed")]
        public async Task<IActionResult> DeleteSurveyConfirmed(int surveyFormId)
        {
            await _surveyService.DeleteSurveyAsync(surveyFormId);
            return RedirectToAction(nameof(Index));
        }

        // GET: Home/CopySurvey/5
        public async Task<IActionResult> CopySurvey(int id)
        {
            var newSurvey = await _surveyService.CopySurveyAsync(id);
            if (newSurvey == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Home/PreviewSurvey
        public IActionResult PreviewSurvey()
        {
            if (TempData["SurveyPreview"] is string surveyJson)
            {
                var survey = JsonConvert.DeserializeObject<SurveyFormViewModel>(surveyJson);
                TempData.Keep("SurveyPreview");
                TempData.Keep("QuestionsToRemove");
                TempData.Keep("OptionsToRemove");
                return View(survey);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Home/CreateSurvey/5 (for returning to edit mode)
        public IActionResult CreateSurveyFromPreview()
        {
            if (TempData["SurveyPreview"] is string surveyJson)
            {
                var survey = JsonConvert.DeserializeObject<SurveyFormViewModel>(surveyJson);
                TempData.Keep("SurveyPreview");
                TempData.Keep("QuestionsToRemove");
                TempData.Keep("OptionsToRemove");
                return View("CreateSurvey", survey);
            }

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}