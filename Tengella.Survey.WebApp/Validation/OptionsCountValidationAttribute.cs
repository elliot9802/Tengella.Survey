using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using Tengella.Survey.Data.Models;

namespace Tengella.Survey.WebApp.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class OptionsCountValidationAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is ICollection<Option> options)
            {
                var questionType = validationContext.ObjectInstance.GetType().GetProperty("Type")?.GetValue(validationContext.ObjectInstance, null)?.ToString();

                if (questionType == "Radio" && (options.Count < 2 || options.Count > 10))
                {
                    return new ValidationResult("Each question must have between 2 and 10 options.");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Options are required.");
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-optionscount", "Each question must have between 2 and 10 options.");
            context.Attributes.Add("data-val-optionscount-min", "2");
            context.Attributes.Add("data-val-optionscount-max", "10");
        }
    }
}
