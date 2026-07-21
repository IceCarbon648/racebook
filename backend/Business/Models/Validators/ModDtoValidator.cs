using Business.Models.DTOs.Request;
using FluentValidation;

namespace Business.Models.Validators
{
    public class ModDtoValidator : AbstractValidator<ModDto>
    {
        private static readonly string[] AllowedImageExtensions = [".png", ".jpg", ".jpeg"];
        private static readonly string[] AllowedImageMimeTypes = ["image/png", "image/jpeg"];
        private const string AllowedModExtension = ".tpf";
        private const string AllowedModMimeType = "application/octet-stream";
        private const long MaxModFileSize = 10 * 1024 * 1024;
        private const long MaxImageSize = 5 * 1024 * 1024;

        public ModDtoValidator()
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
                .Must(f => Path.GetExtension(f.FileName).Equals(AllowedModExtension, StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Mod file must be a .tpf file")
            .Must(f => f.ContentType.Equals(AllowedModMimeType, StringComparison.OrdinalIgnoreCase))
                .WithMessage("Mod file has an invalid MIME type");

            RuleFor(x => x.PreviewImage)
                .NotNull()
                .Must(f => f.Length <= MaxImageSize)
                    .WithMessage("Preview image cannot be larger than 5MB")
                .Must(f => AllowedImageExtensions.Contains(Path.GetExtension(f.FileName).ToLower()))
                    .WithMessage("Preview image must be a .png or .jpg file")
            .Must(f => AllowedImageMimeTypes.Contains(f.ContentType.ToLower()))
                .WithMessage("Preview image has an invalid MIME type");
        }
    }
}