namespace Infrastructure.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<string>> GetCategories();
    }
}