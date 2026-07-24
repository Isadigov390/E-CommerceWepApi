using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Shopping.Application.Validations
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(params string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_extensions.Contains(extension))
                    return new ValidationResult($"Only {string.Join(", ", _extensions)} files are allowed.");
            }
            return ValidationResult.Success;
        }
    }
}