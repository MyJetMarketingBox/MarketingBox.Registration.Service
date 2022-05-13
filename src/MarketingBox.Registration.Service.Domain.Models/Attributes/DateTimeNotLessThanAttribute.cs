using System;
using System.ComponentModel.DataAnnotations;

namespace MarketingBox.Registration.Service.Domain.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateTimeNotLessThanAttribute : ValidationAttribute
    {
        private const string DefaultErrorMessage = "{0} can't be less than {1}.";

        public DateTimeNotLessThanAttribute(string otherProperty, string otherPropertyName)
            : base(DefaultErrorMessage)
        {
            if (string.IsNullOrEmpty(otherProperty)) throw new ArgumentNullException(nameof(otherProperty));

            OtherProperty = otherProperty;
            OtherPropertyName = otherPropertyName;
        }

        private string OtherProperty { get; }
        private string OtherPropertyName { get; }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessageString, name, OtherPropertyName);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var otherProperty = validationContext.ObjectInstance.GetType().GetProperty(OtherProperty);
            var otherPropertyValue = otherProperty?.GetValue(validationContext.ObjectInstance, null);

            var dtThis = Convert.ToDateTime(value);
            var dtOther = Convert.ToDateTime(otherPropertyValue);

            return dtThis < dtOther
                ? new ValidationResult(
                    FormatErrorMessage(validationContext.DisplayName),
                    new[]
                    {
                        validationContext.DisplayName
                    })
                : ValidationResult.Success;
        }
    }
}