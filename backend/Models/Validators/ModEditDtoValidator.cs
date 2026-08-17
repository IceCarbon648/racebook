using Helpers.Interfaces;
using FluentValidation;
using Models.DTOs.Request;
using static Models.Constants.Constants;

namespace Models.Validators
{
    public class ModEditDtoValidator : AbstractValidator<ModEditDto>
    {
        public ModEditDtoValidator(IFileChecker fileChecker)
        {
            RuleFor(x => x.Title)
                .MaximumLength(32)
                .When(x => x.Title != null);

            RuleFor(x => x.Type)
                .MaximumLength(16)
                .Must(x => Types.Contains(x))
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
                    .WithMessage("Invalid mod file")
                .When(x => x.ModFile != null);

            RuleFor(x => x.PreviewImage)
                .Must(f => f!.Length <= MaxImageSize)
                    .WithMessage("Preview image cannot be larger than 5MB")
                .Must(f => fileChecker.HasSafeName(f))
                    .WithMessage("Preview image has an invalid file name")
                .MustAsync(async (f, _) => await fileChecker.IsValidImageAsync(f))
                    .WithMessage("Invalid image file")
                .When(x => x.PreviewImage != null);
        }
    }
}