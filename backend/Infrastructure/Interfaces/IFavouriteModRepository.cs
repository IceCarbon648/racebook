using Infrastructure.Models.DTOs.Response;

namespace Infrastructure.Interfaces
{
    public interface IFavouriteModRepository
    {
        Task AddToFavourites(string uid, string modId);
        Task<List<GetModDto>> GetFavourites(string uid);
        Task DeleteFromFavourites(string uid, string modId);
    }
}