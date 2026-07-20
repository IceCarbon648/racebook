using Business.Models.DTOs.Request;
using FluentValidation;

namespace Business.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(320);

            RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(16);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .Matches("[A-Za-z]").WithMessage("Password must contain at least one letter")
                .Matches("[0-9]").WithMessage("Password must contain at least one number")
                .Matches("[^A-Za-z0-9]").WithMessage("Password must contain at least one special character");
        }
    }
}