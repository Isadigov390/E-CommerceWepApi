using FluentValidation;
using Shopping.Application.DTOs.AccountDTOs.Requests;

namespace Shopping.Application.Validations
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDTO>
    {
        public RegisterRequestValidator()
        {
            RuleFor(m => m.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must be at most 100 characters.");

            RuleFor(m => m.Surname)
                .NotEmpty().WithMessage("Surname is required.")
                .MaximumLength(100).WithMessage("Surname must be at most 100 characters.");

            RuleFor(m => m.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email format is not valid.")
                .MaximumLength(256).WithMessage("Email must be at most 256 characters.");

            RuleFor(m => m.Password)
                            .NotEmpty().WithMessage("Password is required.")
                            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
                            .MaximumLength(64).WithMessage("Password must be at most 64 characters.")
                            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                            .Matches("[0-9]").WithMessage("Password must contain at least one number.")
                            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
        }
    }
}