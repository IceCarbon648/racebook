using Microsoft.AspNetCore.Http;

namespace Business.Helpers.Interfaces
{
    public interface IFileChecker
    {
        Task<bool> IsValidImageAsync(IFormFile file);
        Task<bool> IsValidModAsync(IFormFile file);
        bool HasSafeName(IFormFile file);
    }
}