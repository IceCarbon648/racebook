using Helpers.Interfaces;
using FluentValidation;
using Models.DTOs.Request;

namespace Models.Validators
{
    public class ModDtoValidator : AbstractValidator<ModDto>
    {
        private const long MaxModFileSize = 10 * 1024 * 1024;
        private const long MaxImageSize = 5 * 1024 * 1024;

        public ModDtoValidator(IFileChecker fileChecker)
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(32);

            RuleFor(x => x.Type)
                .NotEmpty()
                .MaximumLength(16);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(128);

            RuleFor(x => x.ModFile)
                .NotNull()
                .Must(f => f.Length <= MaxModFileSize)
                    .WithMessage("Mod file cannot be larger than 10MB")
                .Must(f => fileChecker.HasSafeName(f))
                    .WithMessage("Mod file has an invalid file name")
                .MustAsync(async (f, _) => await fileChecker.IsValidModAsync(f))
                    .WithMessage("Invalid mod file");

            RuleFor(x => x.PreviewImage)
                .NotNull()
                .Must(f => f.Length <= MaxImageSize)
                    .WithMessage("Preview image cannot be larger than 5MB")
                .Must(f => fileChecker.HasSafeName(f))
                    .WithMessage("Preview image has an invalid file name")
                .MustAsync(async (f, _) => await fileChecker.IsValidImageAsync(f))
                    .WithMessage("Invalid image file");
        }
    }
}