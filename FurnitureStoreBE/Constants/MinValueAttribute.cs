using System.ComponentModel.DataAnnotations;

namespace FurnitureStoreBE.Constants
{
    public class MinValueAttribute : ValidationAttribute
    {
        private readonly decimal _minValue;

        public MinValueAttribute(double minValue)
        {
            _minValue = (decimal)minValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && value is decimal decimalValue)
            {
                if (decimalValue <= _minValue)
                {
                    return new ValidationResult(ErrorMessage ?? $"The value must be greater than or equal to {_minValue}.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
