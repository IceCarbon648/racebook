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

namespace racebookApiTests;

[TestFixture]
public class ModServiceTests
{
    private ICloudinaryRepository _cloudinaryRepository;
    private IModRepository _modRepository;
    private ILogger<ModService> _logger;
    private ModService _modService;

    private const string PreviewImagesPublicIdStart = "PreviewImages";
    private const string ModPublicIdStart = "Mods";

    private const string DefaultModFileUrl = "https://cloudinary.com/mod.tpf";
    private const string DefaultImageUrl = "https://cloudinary.com/preview.png";

    [SetUp]
    public void SetUp()
    {
        _cloudinaryRepository = Substitute.For<ICloudinaryRepository>();
        _modRepository = Substitute.For<IModRepository>();
        _logger = Substitute.For<ILogger<ModService>>();
        _modService = new ModService(_cloudinaryRepository, _modRepository, _logger);

        _cloudinaryRepository
            .DeleteAsync(Arg.Any<string>(), Arg.Any<string>())
            .Returns(SuccessfulDeletion());
    }

    private static Mod ExistingMod(
        Guid modId,
        string modFileUrl = DefaultModFileUrl,
        string imageUrl = DefaultImageUrl) =>
        new Mod
        {
            ModId = modId,
            Uid = Guid.NewGuid(),
            Title = "Title",
            Type = "Type",
            Description = "Description",
            UploadDate = DateTime.Now,
            EditDate = DateTime.Now,
            ModFileUrl = modFileUrl,
            ImageUrl = imageUrl
        };

    private static ModDto UploadDto(IFormFile modFile, IFormFile previewImage) =>
        new ModDto
        {
            Title = "Test Mod",
            Type = "TestType",
            Description = "Test Description",
            ModFile = modFile,
            PreviewImage = previewImage
        };

    private static ModEditDto EditDto(
        string title = "Title",
        string type = "Type",
        string description = "Description",
        IFormFile? modFile = null,
        IFormFile? previewImage = null) =>
        new ModEditDto
        {
            Title = title,
            Type = type,
            Description = description,
            ModFile = modFile,
            PreviewImage = previewImage
        };

    private static GetModDto SampleGetModDto(string creator, string title) =>
        new GetModDto
        {
            ModId = Guid.NewGuid(),
            Creator = creator,
            Title = title,
            Type = "Type",
            Description = "Description",
            UploadDate = DateTime.Now,
            EditDate = DateTime.Now,
            ModFileUrl = DefaultModFileUrl,
            PreviewImageUrl = DefaultImageUrl
        };

    private static ModDetailsDto SampleModDetailsDto() =>
        new ModDetailsDto
        {
            Creator = "User1",
            Title = "Mod 1",
            Type = "Type",
            Description = "Description",
            UploadDate = DateTime.Now,
            EditDate = DateTime.Now,
            ModFileUrl = DefaultModFileUrl,
            PreviewImageUrl = DefaultImageUrl
        };

    private static RawUploadResult SuccessfulUpload(string url) =>
        new RawUploadResult
        {
            StatusCode = HttpStatusCode.OK,
            SecureUrl = new Uri(url)
        };

    private static RawUploadResult FailedUpload() =>
        new RawUploadResult
        {
            StatusCode = HttpStatusCode.BadGateway,
            Error = new Error { Message = "Cloudinary unavailable" }
        };

    private static DeletionResult SuccessfulDeletion() =>
        new DeletionResult { StatusCode = HttpStatusCode.OK };

    [Test]
    public async Task GivenValidInput_WhenUploadModIsCalled_ThenModIsCreated()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        IFormFile modFile = Substitute.For<IFormFile>();
        IFormFile previewImage = Substitute.For<IFormFile>();
        ModDto dto = UploadDto(modFile, previewImage);

        string modFileUrl = "https://cloudinary.com/mod.tpf";
        string previewImageUrl = "https://cloudinary.com/preview.png";
        Guid modId = Guid.NewGuid();

        _cloudinaryRepository.UploadAsync(modFile, FileType.Raw).Returns(SuccessfulUpload(modFileUrl));
        _cloudinaryRepository.UploadAsync(previewImage, FileType.Image).Returns(SuccessfulUpload(previewImageUrl));
        _modRepository.CreateMod(Arg.Any<NewMod>()).Returns(modId);

        //Act
        Guid? result = await _modService.UploadMod(uid, dto);

        //Assert
        Assert.That(result, Is.EqualTo(modId));
        await _modRepository.Received(1).CreateMod(Arg.Is<NewMod>(mod =>
            mod.Uid == uid
            && mod.Title == dto.Title
            && mod.Type == dto.Type
            && mod.Description == dto.Description
            && mod.ModFileUrl == modFileUrl
            && mod.ImageUrl == previewImageUrl));
    }

    [Test]
    public async Task GivenFailedModFileUpload_WhenUploadModIsCalled_ThenNullIsReturnedAndModIsNotCreated()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        IFormFile modFile = Substitute.For<IFormFile>();
        IFormFile previewImage = Substitute.For<IFormFile>();
        ModDto dto = UploadDto(modFile, previewImage);

        _cloudinaryRepository.UploadAsync(modFile, FileType.Raw).Returns(FailedUpload());

        //Act
        Guid? result = await _modService.UploadMod(uid, dto);

        //Assert
        Assert.That(result, Is.Null);
        await _cloudinaryRepository.DidNotReceive().UploadAsync(previewImage, FileType.Image);
        await _modRepository.DidNotReceive().CreateMod(Arg.Any<NewMod>());
    }

    [Test]
    public async Task GivenFailedPreviewImageUpload_WhenUploadModIsCalled_ThenOrphanedModFileIsDeletedAndNullIsReturned()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        IFormFile modFile = Substitute.For<IFormFile>();
        IFormFile previewImage = Substitute.For<IFormFile>();
        ModDto dto = UploadDto(modFile, previewImage);

        string modFileUrl = "https://cloudinary.com/mod.tpf";

        _cloudinaryRepository.UploadAsync(modFile, FileType.Raw).Returns(SuccessfulUpload(modFileUrl));
        _cloudinaryRepository.UploadAsync(previewImage, FileType.Image).Returns(FailedUpload());

        //Act
        Guid? result = await _modService.UploadMod(uid, dto);

        //Assert
        Assert.That(result, Is.Null);
        await _cloudinaryRepository.Received(1).DeleteAsync(modFileUrl, ModPublicIdStart);
        await _modRepository.DidNotReceive().CreateMod(Arg.Any<NewMod>());
    }

    [Test]
    public async Task GivenExistingMod_WhenDeleteModIsCalled_ThenModAndFilesAreDeleted()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        Mod mod = ExistingMod(Guid.Parse(modId));

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
    public async Task GivenNewDetails_WhenEditModIsCalled_ThenAllThreeAreCarriedOntoTheMod()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        Mod modDetails = ExistingMod(Guid.Parse(modId));

        ModEditDto dto = EditDto(
            title: "New Title",
            type: "New Type",
            description: "New Description");

        _modRepository.GetModById(modId).Returns(modDetails);

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.True);
        await _modRepository.Received(1).EditMod(Arg.Is<Mod>(m =>
            m.ModId == modDetails.ModId
            && m.Title == dto.Title
            && m.Type == dto.Type
            && m.Description == dto.Description));
    }

    [Test]
    public async Task GivenNoNewFiles_WhenEditModIsCalled_ThenNothingIsUploadedOrDeleted()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        Mod modDetails = ExistingMod(Guid.Parse(modId));

        ModEditDto dto = EditDto(title: "New Title");

        _modRepository.GetModById(modId).Returns(modDetails);

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.True);
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

        Mod modDetails = ExistingMod(Guid.Parse(modId), imageUrl: oldImageUrl);
        ModEditDto dto = EditDto(previewImage: newImage);

        _modRepository.GetModById(modId).Returns(modDetails);
        _cloudinaryRepository.UploadAsync(newImage, FileType.Image).Returns(SuccessfulUpload(newImageUrl));

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.True);
        await _cloudinaryRepository.Received(1).UploadAsync(newImage, FileType.Image);
        await _cloudinaryRepository.Received(1).DeleteAsync(oldImageUrl, PreviewImagesPublicIdStart);
        await _modRepository.Received(1).EditMod(Arg.Is<Mod>(m => m.ImageUrl == newImageUrl));
    }

    [Test]
    public async Task GivenNewModFile_WhenEditModIsCalled_ThenOldFileIsDeletedAndNewFileIsUploaded()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        string oldModFileUrl = "https://cloudinary.com/old-mod.tpf";
        string newModFileUrl = "https://cloudinary.com/new-mod.tpf";
        IFormFile newModFile = Substitute.For<IFormFile>();

        Mod modDetails = ExistingMod(Guid.Parse(modId), modFileUrl: oldModFileUrl);
        ModEditDto dto = EditDto(modFile: newModFile);

        _modRepository.GetModById(modId).Returns(modDetails);
        _cloudinaryRepository.UploadAsync(newModFile, FileType.Raw).Returns(SuccessfulUpload(newModFileUrl));

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.True);
        await _cloudinaryRepository.Received(1).UploadAsync(newModFile, FileType.Raw);
        await _cloudinaryRepository.Received(1).DeleteAsync(oldModFileUrl, ModPublicIdStart);
        await _modRepository.Received(1).EditMod(Arg.Is<Mod>(m => m.ModFileUrl == newModFileUrl));
    }

    [Test]
    public async Task GivenFailedModFileUpload_WhenEditModIsCalled_ThenNothingElseHappensAndFalseIsReturned()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        IFormFile newModFile = Substitute.For<IFormFile>();
        IFormFile newImage = Substitute.For<IFormFile>();

        Mod modDetails = ExistingMod(Guid.Parse(modId));
        ModEditDto dto = EditDto(modFile: newModFile, previewImage: newImage);

        _modRepository.GetModById(modId).Returns(modDetails);
        _cloudinaryRepository.UploadAsync(newModFile, FileType.Raw).Returns(FailedUpload());

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.False);
        await _cloudinaryRepository.DidNotReceive().UploadAsync(newImage, FileType.Image);
        await _cloudinaryRepository.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<string>());
        await _modRepository.DidNotReceive().EditMod(Arg.Any<Mod>());
    }

    [Test]
    public async Task GivenFailedPreviewImageUpload_WhenEditModIsCalled_ThenFalseIsReturnedAndModIsNotEdited()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        string oldImageUrl = "https://cloudinary.com/old-preview.png";
        IFormFile newImage = Substitute.For<IFormFile>();

        Mod modDetails = ExistingMod(Guid.Parse(modId), imageUrl: oldImageUrl);
        ModEditDto dto = EditDto(previewImage: newImage);

        _modRepository.GetModById(modId).Returns(modDetails);
        _cloudinaryRepository.UploadAsync(newImage, FileType.Image).Returns(FailedUpload());

        //Act
        bool result = await _modService.EditMod(modId, dto);

        //Assert
        Assert.That(result, Is.False);
        await _modRepository.DidNotReceive().EditMod(Arg.Any<Mod>());
        await _cloudinaryRepository.DidNotReceive().DeleteAsync(Arg.Any<string>(), Arg.Any<string>());
    }

    [Test]
    public async Task GivenNonExistentMod_WhenEditModIsCalled_ThenArgumentNullExceptionIsThrownAndNothingIsUploaded()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        ModEditDto dto = EditDto(title: "New Title");

        _modRepository.GetModById(modId).Returns((Mod?)null);

        //Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _modService.EditMod(modId, dto));
        await _cloudinaryRepository.DidNotReceive().UploadAsync(Arg.Any<IFormFile>(), Arg.Any<FileType>());
    }

    [Test]
    public async Task GivenExistingMod_WhenGetModDetailsIsCalled_ThenTheModDetailsAreReturned()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();
        ModDetailsDto modDetails = SampleModDetailsDto();

        _modRepository.GetModDetails(modId).Returns(modDetails);

        //Act
        ModDetailsDto result = await _modService.GetModDetails(modId);

        //Assert
        Assert.That(result, Is.EqualTo(modDetails));
    }

    [Test]
    public void GivenNonExistentMod_WhenGetModDetailsIsCalled_ThenArgumentNullExceptionIsThrown()
    {
        //Arrange
        string modId = Guid.NewGuid().ToString();

        _modRepository.GetModDetails(modId).Returns((ModDetailsDto?)null);

        //Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _modService.GetModDetails(modId));
    }

    [Test]
    public async Task GivenModsExist_WhenGetAllModsIsCalled_ThenAllModsAreReturned()
    {
        //Arrange
        string uid = Guid.NewGuid().ToString();
        List<GetModDto> mods =
        [
            SampleGetModDto("User1", "Mod 1"),
            SampleGetModDto("User2", "Mod 2")
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
            ExistingMod(Guid.NewGuid()),
            ExistingMod(Guid.NewGuid())
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