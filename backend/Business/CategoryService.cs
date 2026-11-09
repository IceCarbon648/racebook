using Business.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Business
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ICategoryRepository categoryRepository, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<List<string>> GetCategories()
        {
            _logger.LogInformation("Retrieving all categories");

            return await _categoryRepository.GetCategories();
        }
    }
}