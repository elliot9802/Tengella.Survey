using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tengella.Survey.Business.Interfaces;
using Tengella.Survey.Data.Models;
using Tengella.Survey.WebApp.Models;

namespace Tengella.Survey.WebApp.Controllers
{
    public class SurveyController(ILogger<SurveyController> logger, ISurveyService surveyService) : Controller
    {
        private readonly ILogger<SurveyController> _logger = logger;
        private readonly ISurveyService _surveyService = surveyService;

        // GET: Survey/CreateSurvey/5
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

        // POST: Survey/CreateSurvey
        [HttpPost]
        public async Task<IActionResult> CreateSurvey(SurveyFormViewModel survey, string? questionsToRemove, string? optionsToRemove, bool isPreview = false)
        {
            var questionsToRemoveList = string.IsNullOrEmpty(questionsToRemove) ? [] : questionsToRemove.Split(',').Select(int.Parse).ToList();
            var optionsToRemoveList = string.IsNullOrEmpty(optionsToRemove) ? [] : optionsToRemove.Split(',').Select(int.Parse).ToList();

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

                return RedirectToAction("Index", "Home");
            }

            _logger.LogWarning("ModelState is not valid. Errors: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return View(survey);
        }

        // GET: Survey/DeleteSurvey/5
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return View(survey);
        }

        // POST: Survey/DeleteSurvey/5
        [HttpPost, ActionName("DeleteSurveyConfirmed")]
        public async Task<IActionResult> DeleteSurveyConfirmed(int surveyFormId)
        {
            await _surveyService.DeleteSurveyAsync(surveyFormId);
            return RedirectToAction("Index", "Home");

        }

        // GET: Survey/CopySurvey/5
        public async Task<IActionResult> CopySurvey(int id)
        {
            var newSurvey = await _surveyService.CopySurveyAsync(id);
            if (newSurvey == null)
            {
                return NotFound();
            }
                return RedirectToAction("Index", "Home");
        }

        // GET: Survey/PreviewSurvey
        public async Task<IActionResult> PreviewSurvey(int? id)
        {
            if (id == null)
            {
                if (TempData["SurveyPreview"] is string surveyJson)
                {
                    var survey = JsonConvert.DeserializeObject<SurveyForm>(surveyJson);
                    TempData.Keep("SurveyPreview");
                    TempData.Keep("QuestionsToRemove");
                    TempData.Keep("OptionsToRemove");
                    return View(survey);
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id.Value);
                if (survey == null)
                {
                    return NotFound();
                }
                return View(survey);
            }
        }

        // GET: Survey/CreateSurveyFromPreview (for returning to edit mode)
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

            return RedirectToAction("Index", "Home");
        }
    }
}