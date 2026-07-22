using Business.Helpers.Interfaces;
using Business.Models.DTOs.Request;
using FluentValidation;

namespace Business.Models.Validators
{
    public class ModEditDtoValidator : AbstractValidator<ModEditDto>
    {
        private const long MaxModFileSize = 10 * 1024 * 1024;
        private const long MaxImageSize = 5 * 1024 * 1024;

        public ModEditDtoValidator(IFileChecker fileChecker)
        {
            RuleFor(x => x.Title)
                .MaximumLength(32)
                .When(x => x.Title != null);

            RuleFor(x => x.Type)
                .MaximumLength(16)
                .When(x => x.Type != null);

            RuleFor(x => x.Description)
                .MaximumLength(128)
                .When(x => x.Description != null);

            RuleFor(x => x.ModFile)
                .Must(f => f!.Length <= MaxModFileSize)
                    .WithMessage("Mod file cannot be larger than 10MB")
                .Must(f => fileChecker.HasSafeName(f))
                    .WithMessage("Mod file has an invalid file name")
                .MustAsync(async (f, _) => await fileChecker.IsValidModAsync(f))
                    .WithMessage("Invalid mod file");

            RuleFor(x => x.PreviewImage)
                .Must(f => f!.Length <= MaxImageSize)
                    .WithMessage("Preview image cannot be larger than 5MB")
                .Must(f => fileChecker.HasSafeName(f))
                    .WithMessage("Preview image has an invalid file name")
                .MustAsync(async (f, _) => await fileChecker.IsValidImageAsync(f))
                    .WithMessage("Invalid image file");
        }
    }
}