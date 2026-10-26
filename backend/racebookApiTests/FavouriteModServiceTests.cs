using Business;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Models.DTOs.Response;
using NSubstitute;

namespace racebookApiTests;

[TestFixture]
public class FavouriteModServiceTests
{
    private IFavouriteModRepository _favouriteModRepository;
    private ILogger<FavouriteModService> _logger;
    private FavouriteModService _favouriteModService;

    [SetUp]
    public void SetUp()
    {
        _favouriteModRepository = Substitute.For<IFavouriteModRepository>();
        _logger = Substitute.For<ILogger<FavouriteModService>>();
        _favouriteModService = new FavouriteModService(_favouriteModRepository, _logger);
    }

    [Test]
    public async Task GivenValidInput_WhenAddToFavouritesIsCalled_ThenModIsAddedToFavourites()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        string modId = Guid.NewGuid().ToString();

        //Act
        await _favouriteModService.AddToFavourites(uid, modId);

        //Assert
        await _favouriteModRepository.Received(1).AddToFavourites(uid, modId);
    }

    [Test]
    public async Task GivenValidInput_WhenDeleteFromFavouritesIsCalled_ThenModIsRemovedFromFavourites()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        string modId = Guid.NewGuid().ToString();

        //Act
        await _favouriteModService.DeleteFromFavourites(uid, modId);

        //Assert
        await _favouriteModRepository.Received(1).DeleteFromFavourites(uid, modId);
    }

    [Test]
    public async Task GivenFavouritesExist_WhenGetFavouritesIsCalled_ThenFavouritesAreReturned()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        List<GetModDto> favourites =
        [
            new GetModDto
            {
                ModId = Guid.NewGuid(),
                Creator = "User1",
                Title = "Mod 1",
                Type = "Type",
                Description = "Description",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
                ModFileUrl = "https://cloudinary.com/mod1.tpf",
                PreviewImageUrl = "https://cloudinary.com/preview1.png",
                IsFavourite = true
            },
            new GetModDto
            {
                ModId = Guid.NewGuid(),
                Creator = "User2",
                Title = "Mod 2",
                Type = "Type",
                Description = "Description",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
                ModFileUrl = "https://cloudinary.com/mod2.tpf",
                PreviewImageUrl = "https://cloudinary.com/preview2.png",
                IsFavourite = true
            }
        ];

        _favouriteModRepository.GetFavourites(uid).Returns(favourites);

        //Act
        List<GetModDto> result = await _favouriteModService.GetFavourites(uid);

        //Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result, Is.EqualTo(favourites));
    }

    [Test]
    public async Task GivenNoFavouritesExist_WhenGetFavouritesIsCalled_ThenEmptyListIsReturned()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        _favouriteModRepository.GetFavourites(uid).Returns([]);

        //Act
        List<GetModDto> result = await _favouriteModService.GetFavourites(uid);

        //Assert
        Assert.That(result, Is.Empty);
    }
}