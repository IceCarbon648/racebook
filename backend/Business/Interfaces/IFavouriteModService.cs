using Models.DTOs.Response;

namespace Business.Interfaces
{
    public interface IFavouriteModService
    {
        Task AddToFavourites(string uid, string modId);
        Task<List<GetModDto>> GetFavourites(string uid);
        Task DeleteFromFavourites(string uid, string modId);
    }
}