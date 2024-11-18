using System.ComponentModel.DataAnnotations;

namespace STech.Data.Validations
{
    public class GreaterThan : ValidationAttribute
    {
        private readonly string _otherProperty;

        public GreaterThan(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(_otherProperty);
            if (otherPropertyInfo == null)
            {
                return new ValidationResult($"Property with name {_otherProperty} not found.");
            }

            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance);
            if (value == null || otherValue == null)
            {
                return ValidationResult.Success;
            }

            if (decimal.TryParse(value.ToString(), out decimal thisValue) &&
            decimal.TryParse(otherValue.ToString(), out decimal otherDecimalValue))
            {
                if (thisValue <= otherDecimalValue)
                {
                    string defaultMessage = $"{validationContext.DisplayName} must be greater than {_otherProperty}.";
                    return new ValidationResult(ErrorMessage ?? defaultMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}
