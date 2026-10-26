using System.Net;
using Business;
using CloudinaryDotNet.Actions;
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
    private ILogger<ModService> _logger;
    private ModService _modService;
    const string PreviewImagesPublicIdStart = "PreviewImages";
    const string ModPublicIdStart = "Mods";

    [SetUp]
    public void SetUp()
    {
        _cloudinaryRepository = Substitute.For<ICloudinaryRepository>();
        _modRepository = Substitute.For<IModRepository>();
        _logger = Substitute.For<ILogger<ModService>>();
        _modService = new ModService(_cloudinaryRepository, _modRepository, _logger);
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

        RawUploadResult modFileUploadResult = new RawUploadResult
        {
            StatusCode = HttpStatusCode.OK,
            SecureUrl = new Uri(modFileUrl)
        };

        RawUploadResult previewImageUploadResult = new RawUploadResult
        {
            StatusCode = HttpStatusCode.OK,
            SecureUrl = new Uri(previewImageUrl)
        };

        _cloudinaryRepository.UploadAsync(modFile, FileType.Raw).Returns(modFileUploadResult);
        _cloudinaryRepository.UploadAsync(previewImage, FileType.Image).Returns(previewImageUploadResult);
        _modRepository.CreateMod(
            uid, dto.Title, dto.Type, dto.Description,
            Arg.Any<string>(), Arg.Any<string>(),
            modFileUrl, previewImageUrl)
            .Returns(modId);

        //Act
        Guid? result = await _modService.UploadMod(uid, dto);

        //Assert
        Assert.That(result, Is.EqualTo(modId));
        await _modRepository.Received(1).CreateMod(
            uid, dto.Title, dto.Type, dto.Description,
            Arg.Any<string>(), Arg.Any<string>(),
            modFileUrl, previewImageUrl);
    }

    [Test]
    public async Task GivenFailedModFileUpload_WhenUploadModIsCalled_ThenNullIsReturnedAndModIsNotCreated()
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

        RawUploadResult failedUploadResult = new RawUploadResult
        {
            StatusCode = HttpStatusCode.BadGateway,
            Error = new Error { Message = "Cloudinary unavailable" }
        };

        _cloudinaryRepository.UploadAsync(modFile, FileType.Raw).Returns(failedUploadResult);

        //Act
        Guid? result = await _modService.UploadMod(uid, dto);

        //Assert
        Assert.That(result, Is.Null);
        await _cloudinaryRepository.DidNotReceive().UploadAsync(previewImage, FileType.Image);
        await _modRepository.DidNotReceive().CreateMod(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task GivenFailedPreviewImageUpload_WhenUploadModIsCalled_ThenOrphanedModFileIsDeletedAndNullIsReturned()
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

        RawUploadResult modFileUploadResult = new RawUploadResult
        {
            StatusCode = HttpStatusCode.OK,
            SecureUrl = new Uri(modFileUrl)
        };

        RawUploadResult failedUploadResult = new RawUploadResult
        {
            StatusCode = HttpStatusCode.BadGateway,
            Error = new Error { Message = "Cloudinary unavailable" }
        };

        _cloudinaryRepository.UploadAsync(modFile, FileType.Raw).Returns(modFileUploadResult);
        _cloudinaryRepository.UploadAsync(previewImage, FileType.Image).Returns(failedUploadResult);

        //Act
        Guid? result = await _modService.UploadMod(uid, dto);

        //Assert
        Assert.That(result, Is.Null);
        await _cloudinaryRepository.Received(1).DeleteAsync(modFileUrl, ModPublicIdStart);
        await _modRepository.DidNotReceive().CreateMod(
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(),
            Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
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
        await _modRepository.Received(1).DeleteMod(modId);
        await _cloudinaryRepository.Received(1).DeleteAsync(mod.ImageUrl, PreviewImagesPublicIdStart);
        await _cloudinaryRepository.Received(1).DeleteAsync(mod.ModFileUrl, ModPublicIdStart);
    }

    [Test]
    public async Task GivenNonExistentMod_WhenDeleteModIsCalled_ThenArgumentNullExceptionIsThrownAndNoFilesAreDeleted()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();

        _modRepository.DeleteMod(modId).Returns((Mod?)null);

        //Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _modService.DeleteMod(modId));
        await _cloudinaryRepository.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<string>());
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
        _modRepository.EditMod(
            Arg.Any<Mod>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(1);

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.True);
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
        _modRepository.EditMod(
            Arg.Any<Mod>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(1);

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.True);
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
        _modRepository.EditMod(
            Arg.Any<Mod>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(1);

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.True);
        await _modRepository.Received(1).EditMod(
            Arg.Is<Mod>(m => m.ModId == modDetails.ModId),
            dto.Title,
            dto.Type,
            dto.Description);
    }

    [Test]
    public async Task GivenNoChanges_WhenEditModIsCalled_ThenNothingIsUploadedOrDeleted()
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
        _modRepository.EditMod(
            Arg.Any<Mod>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(1);

        //Act
        await _modService.EditMod(modId, dto);

        //Assert
        await _cloudinaryRepository.DidNotReceive().UploadAsync(Arg.Any<IFormFile>(), Arg.Any<FileType>());
        await _cloudinaryRepository.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task GivenNewPreviewImage_WhenEditModIsCalled_ThenOldImageIsDeletedAndNewImageIsUploaded()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        string oldImageUrl = "https://cloudinary.com/old-preview.png";
        string newImageUrl = "https://cloudinary.com/new-preview.png";
        IFormFile newImage = Substitute.For<IFormFile>();

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

        RawUploadResult uploadResult = new RawUploadResult
        {
            StatusCode = HttpStatusCode.OK,
            SecureUrl = new Uri(newImageUrl)
        };

        _modRepository.GetModById(modId).Returns(modDetails);
        _cloudinaryRepository.UploadAsync(newImage, FileType.Image).Returns(uploadResult);
        _modRepository.EditMod(
            Arg.Any<Mod>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(1);

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.True);
        await _cloudinaryRepository.Received(1).UploadAsync(newImage, FileType.Image);
        await _cloudinaryRepository.Received(1).DeleteAsync(oldImageUrl, PreviewImagesPublicIdStart);
        await _modRepository.Received(1).EditMod(
            Arg.Is<Mod>(m => m.ImageUrl == newImageUrl),
            dto.Title,
            dto.Type,
            dto.Description);
    }

    [Test]
    public async Task GivenNewModFile_WhenEditModIsCalled_ThenOldFileIsDeletedAndNewFileIsUploaded()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        string oldModFileUrl = "https://cloudinary.com/old-mod.tpf";
        string newModFileUrl = "https://cloudinary.com/new-mod.tpf";
        IFormFile newModFile = Substitute.For<IFormFile>();

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

        RawUploadResult uploadResult = new RawUploadResult
        {
            StatusCode = HttpStatusCode.OK,
            SecureUrl = new Uri(newModFileUrl)
        };

        _modRepository.GetModById(modId).Returns(modDetails);
        _cloudinaryRepository.UploadAsync(newModFile, FileType.Raw).Returns(uploadResult);
        _modRepository.EditMod(
            Arg.Any<Mod>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(1);

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.True);
        await _cloudinaryRepository.Received(1).UploadAsync(newModFile, FileType.Raw);
        await _cloudinaryRepository.Received(1).DeleteAsync(oldModFileUrl, ModPublicIdStart);
        await _modRepository.Received(1).EditMod(
            Arg.Is<Mod>(m => m.ModFileUrl == newModFileUrl),
            dto.Title,
            dto.Type,
            dto.Description);
    }

    [Test]
    public async Task GivenNonExistentMod_WhenEditModIsCalled_ThenArgumentNullExceptionIsThrownAndNothingIsUploaded()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        ModEditDto dto = new ModEditDto { Title = "New Title" };

        _modRepository.GetModById(modId).Returns((Mod?)null);

        //Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _modService.EditMod(modId, dto));
        await _cloudinaryRepository.DidNotReceive().UploadAsync(Arg.Any<IFormFile>(), Arg.Any<FileType>());
    }

    [Test]
    public async Task GivenFailedPreviewImageUpload_WhenEditModIsCalled_ThenFalseIsReturnedAndModIsNotEdited()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        string oldImageUrl = "https://cloudinary.com/old-preview.png";
        IFormFile newImage = Substitute.For<IFormFile>();

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

        RawUploadResult failedUploadResult = new RawUploadResult
        {
            StatusCode = HttpStatusCode.BadGateway,
            Error = new Error { Message = "Cloudinary unavailable" }
        };

        _modRepository.GetModById(modId).Returns(modDetails);
        _cloudinaryRepository.UploadAsync(newImage, FileType.Image).Returns(failedUploadResult);

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.False);
        await _modRepository.DidNotReceive().EditMod(
            Arg.Any<Mod>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
        await _cloudinaryRepository.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task GivenFailedModFileUpload_WhenEditModIsCalled_ThenNewPreviewImageIsDeletedAndOldFilesAreKept()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        string oldImageUrl = "https://cloudinary.com/old-preview.png";
        string oldModFileUrl = "https://cloudinary.com/old-mod.tpf";
        string newImageUrl = "https://cloudinary.com/new-preview.png";
        IFormFile newImage = Substitute.For<IFormFile>();
        IFormFile newModFile = Substitute.For<IFormFile>();

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
            ModFileUrl = oldModFileUrl
        };

        ModEditDto dto = new ModEditDto { PreviewImage = newImage, ModFile = newModFile };

        RawUploadResult imageUploadResult = new RawUploadResult
        {
            StatusCode = HttpStatusCode.OK,
            SecureUrl = new Uri(newImageUrl)
        };

        RawUploadResult failedUploadResult = new RawUploadResult
        {
            StatusCode = HttpStatusCode.BadGateway,
            Error = new Error { Message = "Cloudinary unavailable" }
        };

        _modRepository.GetModById(modId).Returns(modDetails);
        _cloudinaryRepository.UploadAsync(newImage, FileType.Image).Returns(imageUploadResult);
        _cloudinaryRepository.UploadAsync(newModFile, FileType.Raw).Returns(failedUploadResult);

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.False);
        await _cloudinaryRepository.Received(1).DeleteAsync(newImageUrl, PreviewImagesPublicIdStart);
        await _cloudinaryRepository.DidNotReceive().DeleteAsync(oldImageUrl, PreviewImagesPublicIdStart);
        await _cloudinaryRepository.DidNotReceive().DeleteAsync(oldModFileUrl, ModPublicIdStart);
        await _modRepository.DidNotReceive().EditMod(
            Arg.Any<Mod>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task GivenNoRowsAffected_WhenEditModIsCalled_ThenNewFilesAreDeletedAndFalseIsReturned()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        string oldImageUrl = "https://cloudinary.com/old-preview.png";
        string newImageUrl = "https://cloudinary.com/new-preview.png";
        IFormFile newImage = Substitute.For<IFormFile>();

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

        RawUploadResult uploadResult = new RawUploadResult
        {
            StatusCode = HttpStatusCode.OK,
            SecureUrl = new Uri(newImageUrl)
        };

        _modRepository.GetModById(modId).Returns(modDetails);
        _cloudinaryRepository.UploadAsync(newImage, FileType.Image).Returns(uploadResult);
        _modRepository.EditMod(
            Arg.Any<Mod>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .Returns(0);

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.False);
        await _cloudinaryRepository.Received(1).DeleteAsync(newImageUrl, PreviewImagesPublicIdStart);
        await _cloudinaryRepository.DidNotReceive().DeleteAsync(oldImageUrl, PreviewImagesPublicIdStart);
    }

    [Test]
    public async Task GivenRepositoryThrows_WhenEditModIsCalled_ThenNewFilesAreDeletedAndExceptionIsRethrown()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        string oldImageUrl = "https://cloudinary.com/old-preview.png";
        string newImageUrl = "https://cloudinary.com/new-preview.png";
        IFormFile newImage = Substitute.For<IFormFile>();

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

        RawUploadResult uploadResult = new RawUploadResult
        {
            StatusCode = HttpStatusCode.OK,
            SecureUrl = new Uri(newImageUrl)
        };

        _modRepository.GetModById(modId).Returns(modDetails);
        _cloudinaryRepository.UploadAsync(newImage, FileType.Image).Returns(uploadResult);
        _modRepository.EditMod(
            Arg.Any<Mod>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
            .ThrowsAsync(new InvalidOperationException("Database unavailable"));

        //Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(async () => await _modService.EditMod(modId, dto));
        await _cloudinaryRepository.Received(1).DeleteAsync(newImageUrl, PreviewImagesPublicIdStart);
        await _cloudinaryRepository.DidNotReceive().DeleteAsync(oldImageUrl, PreviewImagesPublicIdStart);
    }

    [Test]
    public async Task GivenModsExist_WhenGetAllModsIsCalled_ThenAllModsAreReturned()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        List<GetModDto> mods =
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
                PreviewImageUrl = "https://cloudinary.com/preview1.png"
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
                PreviewImageUrl = "https://cloudinary.com/preview2.png"
            }
        ];

        _modRepository.GetAllMods(uid).Returns(mods);

        //Act
        List<GetModDto> result = await _modService.GetAllMods(uid);

        //Assert
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result, Is.EqualTo(mods));
    }

    [Test]
    public async Task GivenNoModsExist_WhenGetAllModsIsCalled_ThenEmptyListIsReturned()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        _modRepository.GetAllMods(uid).Returns([]);

        //Act
        List<GetModDto> result = await _modService.GetAllMods(uid);

        //Assert
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GivenNoUid_WhenGetAllModsIsCalled_ThenNullIsPassedToTheRepository()
    {
        //Arrange
        _modRepository.GetAllMods(null).Returns([]);

        //Act
        List<GetModDto> result = await _modService.GetAllMods(null);

        //Assert
        Assert.That(result, Is.Empty);
        await _modRepository.Received(1).GetAllMods(null);
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

    [Test]
    public async Task GivenUserOwnsMod_WhenIsOwnerIsCalled_ThenTrueIsReturned()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        string uid = Guid.NewGuid().ToString();

        _modRepository.IsOwner(modId, uid).Returns(true);

        //Act
        bool result = await _modService.IsOwner(modId, uid);

        //Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task GivenUserDoesNotOwnMod_WhenIsOwnerIsCalled_ThenFalseIsReturned()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        string uid = Guid.NewGuid().ToString();

        _modRepository.IsOwner(modId, uid).Returns(false);

        //Act
        bool result = await _modService.IsOwner(modId, uid);

        //Assert
        Assert.That(result, Is.False);
    }
}