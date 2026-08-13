using FluentValidation;
using Shopping.Application.DTOs.AccountDTOs.Requests;

namespace Shopping.Application.Validations
{
    public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequestDTO>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.Token).NotEmpty().WithMessage("Reset token is required.");

            RuleFor(x => x.NewPassword).NotEmpty().WithMessage("New password is required.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters.").MaximumLength(64)
                .WithMessage("Password must be at most 64 characters.").Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.").Matches("[0-9]").WithMessage("Password must contain a number.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain a special character.");

            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Password confirmation is required.")
                .Equal(x => x.NewPassword).WithMessage("Passwords do not match.");
        }
    }
}