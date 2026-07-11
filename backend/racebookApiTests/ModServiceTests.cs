using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Infrastructure.Constants;
using Infrastructure.Models;
using Business.Models.DTOs.Request;
using Business.Models.DTOs.Response;
using Infrastructure.Interfaces;
using Business;

namespace racebookApiTests
{
    public class ModServiceTests
    {
        private ICloudinaryRepository _cloudinaryRepository;
        private IModRepository _modRepository;
        private IPreviewImageRepository _previewImageRepository;
        private IUserRepository _userRepository;
        private IFormFile _formFile;

        private const string PlaceholderModUrl = "https://res.cloudinary.com/XXXXXXXXX/raw/upload/vXXXXXXXXXX/Mods/XXXXXXXXXXXXXXXXXXXX.tpf";
        private const string PlaceholderImageUrl = "https://res.cloudinary.com/XXXXXXXXX/raw/upload/vXXXXXXXXXX/PreviewImages/XXXXXXXXXXXXXXXXXXXX.jpg";
        private static Guid genericModId = Guid.NewGuid();
        private static Guid genericUserId = Guid.NewGuid();

        Mod genericMod = new Mod
        {
            ModId = genericModId,
            Uid = genericUserId,
            Title = "snow brighton",
            Type = "environment",
            Description = "yet another mod from sidali",
            FilePath = PlaceholderModUrl,
            EditDate = DateTime.Now,
            UploadDate = DateTime.Now,
        };

        GetModDto genericGetModDto = new GetModDto
        {
            Id = genericModId.ToString(),
            Creator = "Sidali",
            Title = "snow brighton",
            Type = "environment",
            Description = "yet another mod from sidali",
            UploadDate = DateTime.Now,
            EditDate = DateTime.Now,
            ModFileUrl = PlaceholderModUrl,
            PreviewImageUrls = new List<string> { PlaceholderImageUrl }
        };

        [SetUp]
        public void Setup()
        {
            _cloudinaryRepository = Substitute.For<ICloudinaryRepository>();
            _modRepository = Substitute.For<IModRepository>();
            _previewImageRepository = Substitute.For<IPreviewImageRepository>();
            _userRepository = Substitute.For<IUserRepository>();
            _formFile = Substitute.For<IFormFile>();
        }

        [Test]
        public async Task GivenModDetails_WhenUploading_StoreAllInformationInTheRelevantRepositories()
        {
            //Arrange
            List<IFormFile> previewImages = new List<IFormFile>();

            for (int i = 0; i < 3; i++)
            {
                previewImages.Add(_formFile);
            }

            ModDto dto = new ModDto
            {
                Title = "custom skyboxes",
                Type = "Skybox",
                Description = "make sky colourful",
                ModFile = _formFile,
                PreviewImages = previewImages
            };

            _cloudinaryRepository.UploadAsync(dto.ModFile, FileType.Raw).Returns(PlaceholderModUrl);

            foreach (IFormFile previewImage in previewImages)
            {
                _cloudinaryRepository.UploadAsync(previewImage, FileType.Image).Returns(PlaceholderImageUrl);
            }

            _modRepository.CreateMod(
                "9D51DE57-A958-4B74-B975-52A5F81C7F93",
                dto.Title,
                dto.Type,
                dto.Description,
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                PlaceholderModUrl
                ).Returns(Guid.NewGuid());

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.UploadMod(genericUserId.ToString(), dto);

            //Assert
            await _cloudinaryRepository.Received(1).UploadAsync(dto.ModFile, FileType.Raw);
            await _cloudinaryRepository.Received(3).UploadAsync(_formFile, FileType.Image);
            await _modRepository.Received(1).CreateMod(
                "9D51DE57-A958-4B74-B975-52A5F81C7F93",
                dto.Title,
                dto.Type,
                dto.Description,
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                PlaceholderModUrl
                );
        }

        [Test]
        public async Task GivenAModId_WhenDeleting_DiscardAllInformationAndFilesAssociatedWithIt()
        {
            //Arrange
            List<string> imageUrls = new List<string>{
                PlaceholderImageUrl,
                PlaceholderImageUrl
            };

            _previewImageRepository.GetPreviewImageUrl(genericModId.ToString()).Returns(imageUrls);
            _modRepository.GetModFileUrl(genericModId.ToString()).Returns(PlaceholderModUrl);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.DeleteMod(genericModId.ToString());

            //Assert
            await _cloudinaryRepository.Received(3).DeleteAsync(Arg.Any<DeletionParams>());
            await _previewImageRepository.Received(1).DeletePreviewImageByModId(genericModId.ToString());
            await _modRepository.Received(1).DeleteMod(genericModId.ToString());
        }

        [Test]
        public async Task GivenModId_WhenGettingModDetails_ReturnAllInformationAssociatedWithTheId()
        {
            //Arrange
            Mod genericMod = new Mod
            {
                ModId = genericModId,
                Uid = genericUserId,
                Title = "snow brighton",
                Type = "environment",
                Description = "yet another mod from sidali",
                FilePath = PlaceholderModUrl,
                EditDate = DateTime.Now,
                UploadDate = DateTime.Now,
            };

            GetModDto expectedResult = genericGetModDto;

            _modRepository.GetModById(genericModId.ToString()).Returns(genericMod);
            _userRepository.GetUsernameByUserId(genericUserId.ToString()).Returns("Sidali");
            _previewImageRepository.GetPreviewImageUrl(genericModId.ToString()).Returns(new List<string> { PlaceholderImageUrl });

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            GetModDto actualResult = await modService.GetMod(genericModId.ToString());

            //Assert
            Assert.That(actualResult.Id, Is.EqualTo(expectedResult.Id));
            Assert.That(actualResult.Creator, Is.EqualTo(expectedResult.Creator));
            Assert.That(actualResult.Title, Is.EqualTo(expectedResult.Title));
            Assert.That(actualResult.Type, Is.EqualTo(expectedResult.Type));
            Assert.That(actualResult.Description, Is.EqualTo(expectedResult.Description));
            Assert.That(actualResult.ModFileUrl, Is.EqualTo(expectedResult.ModFileUrl));
            Assert.That(actualResult.PreviewImageUrls, Is.EqualTo(expectedResult.PreviewImageUrls));

            await _modRepository.Received(1).GetModById(Arg.Any<string>());
            await _userRepository.Received(1).GetUsernameByUserId(Arg.Any<string>());
            await _previewImageRepository.Received(1).GetPreviewImageUrl(Arg.Any<string>());
        }

        [Test]
        public async Task GivenNoModId_WhenGettingsMods_ReturnAllMods()
        {
            //Arrange
            List<Guid> modIds = new List<Guid>()
            {
                genericModId,
                genericModId,
            };

            GetModDto mod = genericGetModDto;

            List<GetModDto> expectedResult = new List<GetModDto>
            {
                mod,
                mod
            };

            _modRepository.GetAllModIds().Returns(modIds);

            foreach (Guid modId in modIds)
            {
                _modRepository.GetModById(modId.ToString()).Returns(genericMod);
                _userRepository.GetUsernameByUserId(genericUserId.ToString()).Returns("Sidali");
                _previewImageRepository.GetPreviewImageUrl(modId.ToString()).Returns(new List<string> { PlaceholderImageUrl });
            }

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.GetAllMods();

            //Assert
            await _modRepository.Received(2).GetModById(Arg.Any<string>());
            await _userRepository.Received(2).GetUsernameByUserId(Arg.Any<string>());
            await _previewImageRepository.Received(2).GetPreviewImageUrl(Arg.Any<string>());
        }

        [Test]
        public async Task GivenUserId_WhenGettingsMods_ReturnOnlyModsAssociatedWithUserId()
        {
            //Arrange
            List<Guid> modIds = new List<Guid>()
            {
                genericModId,
                genericModId,
            };

            GetModDto mod = genericGetModDto;

            List<GetModDto> expectedResult = new List<GetModDto>{
                mod,
                mod
            };

            _modRepository.GetMyModIds(genericUserId.ToString()).Returns(modIds);

            foreach (Guid modId in modIds)
            {
                _modRepository.GetModById(modId.ToString()).Returns(genericMod);
                _userRepository.GetUsernameByUserId(genericUserId.ToString()).Returns("Sidali");
                _previewImageRepository.GetPreviewImageUrl(modId.ToString()).Returns(new List<string> { PlaceholderImageUrl });
            }

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.GetMyMods(genericUserId.ToString());

            //Assert
            await _modRepository.Received(2).GetModById(Arg.Any<string>());
            await _userRepository.Received(2).GetUsernameByUserId(Arg.Any<string>());
            await _previewImageRepository.Received(2).GetPreviewImageUrl(Arg.Any<string>());
        }

        [Test]
        public async Task GivenANotNullValueForTitle_WhenEditing_WriteTheChangesToDb()
        {
            //Arrange
            ModEditDto modEdit = new ModEditDto
            {
                Title = "edited Title",
            };

            _modRepository.GetModById(genericModId.ToString()).Returns(genericMod);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.EditMod(genericModId.ToString(), modEdit);

            //Assert
            await _modRepository.Received(1).EditMod(Arg.Any<Mod>());
        }

        [Test]
        public async Task GivenANotNullValueForType_WhenEditing_WriteTheChangesToDb()
        {
            //Arrange
            ModEditDto modEdit = new ModEditDto
            {
                Type = "edited Type",
            };

            _modRepository.GetModById(genericModId.ToString()).Returns(genericMod);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.EditMod(genericModId.ToString(), modEdit);

            //Assert
            await _modRepository.Received(1).EditMod(genericMod);
        }

        [Test]
        public async Task GivenANotNullValueForDescription_WhenEditing_WriteTheChangesToDb()
        {
            //Arrange
            ModEditDto modEdit = new ModEditDto
            {
                Description = "edited description",
            };

            _modRepository.GetModById(genericModId.ToString()).Returns(genericMod);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.EditMod(genericModId.ToString(), modEdit);

            //Assert
            await _modRepository.Received(1).EditMod(Arg.Any<Mod>());
        }

        [Test]
        public async Task GivenANotNullValueForModFile_WhenEditing_WriteTheChangesToDb()
        {
            //Arrange
            ModEditDto modEdit = new ModEditDto
            {
                ModFile = _formFile,
            };

            _modRepository.GetModById(genericModId.ToString()).Returns(genericMod);
            _modRepository.GetModFileUrl(genericModId.ToString()).Returns(PlaceholderModUrl);
            _cloudinaryRepository.UploadAsync(modEdit.ModFile, FileType.Raw).Returns(PlaceholderModUrl);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.EditMod(genericModId.ToString(), modEdit);

            //Assert
            await _modRepository.Received(1).EditMod(Arg.Any<Mod>());
        }

        [Test]
        public async Task GivenANotNullValueForImagesToBeDeleted_WhenEditing_WriteTheChangesToDb()
        {
            //Arrange
            ModEditDto modEdit = new ModEditDto
            {
                PreviewImagesToBeDeleted = new List<string> { PlaceholderImageUrl },
            };

            _modRepository.GetModById(genericModId.ToString()).Returns(genericMod);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.EditMod(genericModId.ToString(), modEdit);

            //Assert
            await _modRepository.Received(1).EditMod(Arg.Any<Mod>());
            await _cloudinaryRepository.Received(1).DeleteAsync(Arg.Any<DeletionParams>());
            await _previewImageRepository.Received(1).DeletePreviewImageByUrl(Arg.Any<string>());
        }

        [Test]
        public async Task GivenANotNullValueForNewImages_WhenEditing_WriteTheChangesToDb()
        {
            //Arrange
            ModEditDto modEdit = new ModEditDto
            {
                NewPreviewImages = new List<IFormFile> { _formFile },
            };

            _modRepository.GetModById(genericModId.ToString()).Returns(genericMod);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.EditMod(genericModId.ToString(), modEdit);

            //Assert
            await _modRepository.Received(1).EditMod(Arg.Any<Mod>());
            await _cloudinaryRepository.Received(1).UploadAsync(Arg.Any<IFormFile>(), FileType.Image);
            await _previewImageRepository.Received(1).CreatePreviewImage(Arg.Any<Guid>(), Arg.Any<string>());
        }
    }
}