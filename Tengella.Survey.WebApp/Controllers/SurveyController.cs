using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
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
        public async Task<IActionResult> CreateSurvey(int? id, bool fromHomePage = false)
        {
            if (fromHomePage)
            {
                HttpContext.Session.Remove("SurveyPreview");
                HttpContext.Session.Remove("QuestionsToRemove");
                HttpContext.Session.Remove("OptionsToRemove");

                return View(new SurveyFormViewModel());
            }

            SurveyFormViewModel? surveyViewModel;

            if (id == null && HttpContext.Session.TryGetValue("SurveyPreview", out byte[]? surveyData))
            {
                var surveyJson = Encoding.UTF8.GetString(surveyData);
                surveyViewModel = JsonConvert.DeserializeObject<SurveyFormViewModel>(surveyJson) ?? new SurveyFormViewModel();
                return View(surveyViewModel);
            }

            var survey = await _surveyService.GetSurveyByIdAsync(id.Value);
            if (survey == null)
            {
                return NotFound();
            }

            surveyViewModel = new SurveyFormViewModel
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
                    Options = q.Options.Select(o => new Option
                    {
                        OptionId = o.OptionId,
                        Text = o.Text,
                        QuestionId = o.QuestionId
                    }).ToList()
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
                    HttpContext.Session.SetString("SurveyPreview", JsonConvert.SerializeObject(survey));
                    HttpContext.Session.SetString("QuestionsToRemove", questionsToRemove ?? string.Empty);
                    HttpContext.Session.SetString("OptionsToRemove", optionsToRemove ?? string.Empty);
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
        [HttpPost]
        public async Task<IActionResult> DeleteSurvey(int id)
        {
            await _surveyService.DeleteSurveyAsync(id);
            return RedirectToAction(nameof(Index), "Home");
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

        // GET: Survey/PreviewSurvey/5
        public async Task<IActionResult> PreviewSurvey(int? id)
        {
            SurveyFormViewModel? survey;

            if (id == null)
            {
                if (HttpContext.Session.TryGetValue("SurveyPreview", out byte[]? surveyData))
                {
                    var surveyJson = Encoding.UTF8.GetString(surveyData);
                    survey = JsonConvert.DeserializeObject<SurveyFormViewModel>(surveyJson) ?? new SurveyFormViewModel();
                    return View(survey);
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                var surveyEntity = await _surveyService.GetSurveyByIdAsync(id.Value);
                if (surveyEntity == null)
                {
                    return NotFound();
                }

                survey = new SurveyFormViewModel
                {
                    SurveyFormId = surveyEntity.SurveyFormId,
                    Name = surveyEntity.Name,
                    Type = surveyEntity.Type,
                    ClosingDate = surveyEntity.ClosingDate,
                    Questions = surveyEntity.Questions.Select(q => new QuestionViewModel
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

                HttpContext.Session.SetString("SurveyPreview", JsonConvert.SerializeObject(survey));
                return View(survey);
            }
        }
    }
}