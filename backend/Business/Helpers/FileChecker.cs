using Business.Helpers.Interfaces;
using MagicBytesValidator.Models;
using MagicBytesValidator.Services;
using Microsoft.AspNetCore.Http;

namespace Business.Helpers
{
    public class FileChecker : IFileChecker
    {
        private readonly IValidator _validator;

        public FileChecker(IValidator validator)
        {
            _validator = validator;
        }

        public async Task<bool> IsValidImageAsync(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName).TrimStart('.').ToLower();
            IFileType? fileType = _validator.Mapping.FindByExtension(extension);

            if (fileType is null) return false;

            using var stream = file.OpenReadStream();
            return await _validator.IsValidAsync(stream, fileType, CancellationToken.None);
        }

        public async Task<bool> IsValidModAsync(IFormFile file)
        {
            IFileType? fileType = _validator.Mapping.FindByExtension("tpf");

            if (fileType is null) return false;

            using Stream stream = file.OpenReadStream();
            return await _validator.IsValidAsync(stream, fileType, CancellationToken.None);
        }

        public bool HasSafeName(IFormFile file)
        {
            string[] ExecutableFileExtensions =
            [
                "exe",
                "dll",
                "bat",
                "cmd",
                "sh",
                "ps1",
                "msi",
                "vbs",
                "js",
                "jar",
                "com",
                "scr"
            ];

            string? fileName = Path.GetFileNameWithoutExtension(file.FileName);
            string[] segments = fileName.Split('.');
            return !segments.Any(s => ExecutableFileExtensions.Contains(s.ToLower()));
        }
    }
}