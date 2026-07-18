using Business.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Models.DTOs.Response;
using System.Formats.Asn1;

namespace Business
{
    public class FavouriteModService : IFavouriteModService
    {
        private readonly IFavouriteModRepository _favouriteModRepository;

        public FavouriteModService(IFavouriteModRepository favouriteModRepository)
        {
            _favouriteModRepository = favouriteModRepository;
        }

        public async Task AddToFavourites(string uid, string modId)
        {
            await _favouriteModRepository.AddToFavourites(uid, modId);
        }
        public async Task<List<GetModDto>> GetFavourites(string uid)
        {
            return await _favouriteModRepository.GetFavourites(uid);
        }

        public async Task DeleteFromFavourites(string uid, string modId)
        {
            await _favouriteModRepository.DeleteFromFavourites(uid, modId);
        }
    }
}