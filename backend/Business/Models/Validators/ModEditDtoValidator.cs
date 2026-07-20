using Business.Models.DTOs.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Models.Validators
{
    public class ModEditDtoValidator : AbstractValidator<ModEditDto>
    {
        private static readonly string[] AllowedImageExtensions = [".png", ".jpg", ".jpeg"];
        private const string AllowedModExtension = ".tpf";
        private const long MaxModFileSize = 10 * 1024 * 1024;
        private const long MaxImageSize = 5 * 1024 * 1024;

        public ModEditDtoValidator()
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
                .Must(f => Path.GetExtension(f!.FileName).Equals(AllowedModExtension, StringComparison.OrdinalIgnoreCase))
                    .WithMessage("Mod file must be a .tpf file")
                .When(x => x.ModFile != null);

            RuleFor(x => x.PreviewImage)
                .Must(f => f!.Length <= MaxImageSize)
                    .WithMessage("Preview image cannot be larger than 5MB")
                .Must(f => AllowedImageExtensions.Contains(Path.GetExtension(f!.FileName).ToLower()))
                    .WithMessage("Preview image must be a .png or .jpg file")
                .When(x => x.PreviewImage != null);
        }
    }
}