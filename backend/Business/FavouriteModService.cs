using Business.Interfaces;
using Infrastructure.Interfaces;
using Infrastructure.Models.DTOs.Response;
using Microsoft.Extensions.Logging;

namespace Business
{
    public class FavouriteModService : IFavouriteModService
    {
        private readonly IFavouriteModRepository _favouriteModRepository;
        private readonly ILogger<FavouriteModService> _logger;

        public FavouriteModService(IFavouriteModRepository favouriteModRepository, ILogger<FavouriteModService> logger)
        {
            _favouriteModRepository = favouriteModRepository;
            _logger = logger;
        }

        public async Task AddToFavourites(string uid, string modId)
        {
            _logger.LogInformation("User {Uid} adding mod {ModId} to favourites", uid, modId);
            await _favouriteModRepository.AddToFavourites(uid, modId);
            _logger.LogInformation("User {Uid} successfully added mod {ModId} to favourites", uid, modId);
        }
        public async Task<List<GetModDto>> GetFavourites(string uid)
        {
            _logger.LogInformation("Retrieving favourites for user {Uid}", uid);
            List<GetModDto> favourites = await _favouriteModRepository.GetFavourites(uid);

            if (favourites.Count == 0)
            {
                _logger.LogInformation("No favourites found for user {Uid}", uid);
            }
            else
            {
                _logger.LogInformation("Retrieved {Count} favourites for user {Uid}", favourites.Count, uid);
            }

            return favourites;
        }

        public async Task DeleteFromFavourites(string uid, string modId)
        {
            _logger.LogInformation("User {Uid} removing mod {ModId} from favourites", uid, modId);
            await _favouriteModRepository.DeleteFromFavourites(uid, modId);
            _logger.LogInformation("User {Uid} successfully removed mod {ModId} from favourites", uid, modId);
        }
    }
}