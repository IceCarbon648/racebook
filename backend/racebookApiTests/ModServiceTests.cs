using Business;
using Infrastructure.Constants;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs.Request;
using Models.DTOs.Response;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace racebookApiTests;

[TestFixture]
public class ModServiceTests
{
    private ICloudinaryRepository _cloudinaryRepository;
    private IModRepository _modRepository;
    private IFavouriteModRepository _favouriteModRepository;
    private ILogger<ModService> _logger;
    private ModService _modService;

    [SetUp]
    public void SetUp()
    {
        _cloudinaryRepository = Substitute.For<ICloudinaryRepository>();
        _modRepository = Substitute.For<IModRepository>();
        _favouriteModRepository = Substitute.For<IFavouriteModRepository>();
        _logger = Substitute.For<ILogger<ModService>>();
        _modService = new ModService(_cloudinaryRepository, _modRepository, _favouriteModRepository, _logger);
    }

    [Test]
    public async Task GivenValidInput_WhenUploadModIsCalled_ThenModIsCreated()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        IFormFile modFile = Substitute.For<IFormFile>();
        IFormFile previewImage = Substitute.For<IFormFile>();

        ModDto dto = new ModDto
        {
            Title = "Test Mod",
            Type = "TestType",
            Description = "Test Description",
            ModFile = modFile,
            PreviewImage = previewImage
        };

        string modFileUrl = "https://cloudinary.com/mod.tpf";
        string previewImageUrl = "https://cloudinary.com/preview.png";
        Guid modId = Guid.NewGuid();

        _cloudinaryRepository.UploadAsync(modFile, FileType.Raw).Returns(modFileUrl);
        _cloudinaryRepository.UploadAsync(previewImage, FileType.Image).Returns(previewImageUrl);
        _modRepository.CreateMod(
            uid, dto.Title, dto.Type, dto.Description,
            Arg.Any<string>(), Arg.Any<string>(),
            modFileUrl, previewImageUrl)
            .Returns(modId);

        //Act
        await _modService.UploadMod(uid, dto);

        //Assert
        await _modRepository.Received(1).CreateMod(
            uid, dto.Title, dto.Type, dto.Description,
            Arg.Any<string>(), Arg.Any<string>(),
            modFileUrl, previewImageUrl);
    }

    [Test]
    public async Task GivenExistingMod_WhenDeleteModIsCalled_ThenModAndFilesAreDeleted()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        Mod mod = new Mod
        {
            ModId = Guid.Parse(modId),
            Uid = Guid.NewGuid(),
            Title = "Test Mod",
            Type = "TestType",
            Description = "Test Description",
            UploadDate = DateTime.Now,
            EditDate = DateTime.Now,
            ImageUrl = "https://cloudinary.com/preview.png",
            ModFileUrl = "https://cloudinary.com/mod.tpf"
        };

        _modRepository.DeleteMod(modId).Returns(mod);

        //Act
        await _modService.DeleteMod(modId);

        //Assert
        await _favouriteModRepository.Received(1).DeleteFavouriteModReference(modId);
        await _modRepository.Received(1).DeleteMod(modId);
        await _cloudinaryRepository.Received(1).DeleteAsync(mod.ImageUrl, "PreviewImages");
        await _cloudinaryRepository.Received(1).DeleteAsync(mod.ModFileUrl, "Mods");
    }

    [Test]
    public async Task GivenNonExistentMod_WhenDeleteModIsCalled_ThenKeyNotFoundExceptionIsThrown()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();

        _modRepository.DeleteMod(modId).Throws(new KeyNotFoundException($"Mod {modId} not found"));

        //Act & Assert
        Assert.ThrowsAsync<KeyNotFoundException>(async () => await _modService.DeleteMod(modId));
    }

    [Test]
    public async Task GivenNewTitle_WhenEditModIsCalled_ThenTitleIsUpdated()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        Mod modDetails = new Mod
        {
            ModId = Guid.Parse(modId),
            Uid = Guid.NewGuid(),
            Title = "Title",
            Type = "Type",
            Description = "Description",
            UploadDate = DateTime.Now,
            EditDate = DateTime.Now,
            ImageUrl = "https://cloudinary.com/preview.png",
            ModFileUrl = "https://cloudinary.com/mod.tpf"
        };

        ModEditDto dto = new ModEditDto { Title = "New Title" };

        _modRepository.GetModById(modId).Returns(modDetails);

        //Act
        await _modService.EditMod(modId, dto);

        //Assert
        await _modRepository.Received(1).EditMod(
            Arg.Is<Mod>(m => m.ModId == modDetails.ModId),
            dto.Title,
            dto.Type,
            dto.Description);
    }

    [Test]
    public async Task GivenNewType_WhenEditModIsCalled_ThenTypeIsUpdated()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        Mod modDetails = new Mod
        {
            ModId = Guid.Parse(modId),
            Uid = Guid.NewGuid(),
            Title = "Title",
            Type = "Type",
            Description = "Description",
            UploadDate = DateTime.Now,
            EditDate = DateTime.Now,
            ImageUrl = "https://cloudinary.com/preview.png",
            ModFileUrl = "https://cloudinary.com/mod.tpf"
        };

        ModEditDto dto = new ModEditDto { Type = "New Type" };

        _modRepository.GetModById(modId).Returns(modDetails);

        //Act
        await _modService.EditMod(modId, dto);

        //Assert
        await _modRepository.Received(1).EditMod(
            Arg.Is<Mod>(m => m.ModId == modDetails.ModId),
            dto.Title,
            dto.Type,
            dto.Description);
    }

    [Test]
    public async Task GivenNewDescription_WhenEditModIsCalled_ThenDescriptionIsUpdated()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        Mod modDetails = new Mod
        {
            ModId = Guid.Parse(modId),
            Uid = Guid.NewGuid(),
            Title = "Title",
            Type = "Type",
            Description = "Description",
            UploadDate = DateTime.Now,
            EditDate = DateTime.Now,
            ImageUrl = "https://cloudinary.com/preview.png",
            ModFileUrl = "https://cloudinary.com/mod.tpf"
        };

        ModEditDto dto = new ModEditDto { Description = "New Description" };

        _modRepository.GetModById(modId).Returns(modDetails);

        //Act
        await _modService.EditMod(modId, dto);

        //Assert
        await _modRepository.Received(1).EditMod(
            Arg.Is<Mod>(m => m.ModId == modDetails.ModId),
            dto.Title,
            dto.Type,
            dto.Description);
    }

    [Test]
    public async Task GivenNewPreviewImage_WhenEditModIsCalled_ThenOldImageIsDeletedAndNewImageIsUploaded()
    {
        // Arrange
        string modId = Guid.NewGuid().ToString();
        string oldImageUrl = "https://cloudinary.com/old-preview.png";
        string newImageUrl = "https://cloudinary.com/new-preview.png";
        IFormFile newImage = Substitute.For<IFormFile>();

        const string PreviewImagesPublicIdStart = "PreviewImages";

        Mod modDetails = new Mod
        {
            ModId = Guid.Parse(modId),
            Uid = Guid.NewGuid(),
            Title = "Title",
            Type = "Type",
            Description = "Description",
            UploadDate = DateTime.Now,
            EditDate = DateTime.Now,
            ImageUrl = oldImageUrl,
            ModFileUrl = "https://cloudinary.com/mod.tpf"
        };

        ModEditDto dto = new ModEditDto { PreviewImage = newImage };

        _modRepository.GetModById(modId).Returns(modDetails);
        _cloudinaryRepository.UploadAsync(newImage, FileType.Image).Returns(newImageUrl);

        // Act
        await _modService.EditMod(modId, dto);

        // Assert
        await _cloudinaryRepository.Received(1).UploadAsync(newImage, FileType.Image);
        await _cloudinaryRepository.Received(1).DeleteAsync(oldImageUrl, PreviewImagesPublicIdStart);
    }

    [Test]
    public async Task GivenNewModFile_WhenEditModIsCalled_ThenOldFileIsDeletedAndNewFileIsUploaded()
    {
        // Arrange
        string modId = Guid.NewGuid().ToString();
        string oldModFileUrl = "https://cloudinary.com/old-mod.tpf";
        string newModFileUrl = "https://cloudinary.com/new-mod.tpf";
        IFormFile newModFile = Substitute.For<IFormFile>();

        const string ModPublicIdStart = "Mods";

        Mod modDetails = new Mod
        {
            ModId = Guid.Parse(modId),
            Uid = Guid.NewGuid(),
            Title = "Title",
            Type = "Type",
            Description = "Description",
            UploadDate = DateTime.Now,
            EditDate = DateTime.Now,
            ImageUrl = "https://cloudinary.com/preview.png",
            ModFileUrl = oldModFileUrl
        };

        ModEditDto dto = new ModEditDto { ModFile = newModFile };

        _modRepository.GetModById(modId).Returns(modDetails);
        _cloudinaryRepository.UploadAsync(newModFile, FileType.Raw).Returns(newModFileUrl);

        // Act
        await _modService.EditMod(modId, dto);

        // Assert
        await _cloudinaryRepository.Received(1).UploadAsync(newModFile, FileType.Raw);
        await _cloudinaryRepository.Received(1).DeleteAsync(oldModFileUrl, ModPublicIdStart);
    }

    [Test]
    public async Task GivenModsExist_WhenGetAllModsIsCalled_ThenAllModsAreReturned()
    {
        // Arrange
        List<GetModDto> mods =
        [
            new GetModDto
            {
                Creator = "User1",
                Title = "Mod 1",
                Type = "Type",
                Description = "Description",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
                ModFileUrl = "https://cloudinary.com/mod1.tpf",
                PreviewImageUrl = "https://cloudinary.com/preview1.png"
            },
            new GetModDto
            {
                Creator = "User2",
                Title = "Mod 2",
                Type = "Type",
                Description = "Description",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
                ModFileUrl = "https://cloudinary.com/mod2.tpf",
                PreviewImageUrl = "https://cloudinary.com/preview2.png"
            }
        ];

        _modRepository.GetAllMods().Returns(mods);

        // Act
        List<GetModDto> result = await _modService.GetAllMods();

        // Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result, Is.EqualTo(mods));
    }

    [Test]
    public async Task GivenNoModsExist_WhenGetAllModsIsCalled_ThenEmptyListIsReturned()
    {
        //Arrange
        _modRepository.GetAllMods().Returns([]);

        //Act
        List<GetModDto> result = await _modService.GetAllMods();

        //Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenUserHasMods_WhenGetMyModsIsCalled_ThenUsersModsAreReturned()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        List<Mod> mods =
        [
            new Mod
            {
                ModId = Guid.NewGuid(),
                Uid = Guid.NewGuid(),
                Title = "My Mod 1",
                Type = "Type",
                Description = "Description",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
                ModFileUrl = "https://cloudinary.com/mod1.tpf",
                ImageUrl = "https://cloudinary.com/preview1.png"
            },
            new Mod
            {
                ModId = Guid.NewGuid(),
                Uid = Guid.NewGuid(),
                Title = "My Mod 2",
                Type = "Type",
                Description = "Description",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
                ModFileUrl = "https://cloudinary.com/mod2.tpf",
                ImageUrl = "https://cloudinary.com/preview2.png"
            }
        ];

        _modRepository.GetMyMods(uid).Returns(mods);

        //Act
        List<Mod> result = await _modService.GetMyMods(uid);

        //Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result, Is.EqualTo(mods));
    }

    [Test]
    public async Task GivenUserHasNoMods_WhenGetMyModsIsCalled_ThenEmptyListIsReturned()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        _modRepository.GetMyMods(uid).Returns([]);

        //Act
        List<Mod> result = await _modService.GetMyMods(uid);

        //Assert
        Assert.That(result, Is.Empty);
    }
}