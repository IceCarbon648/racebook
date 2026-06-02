using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using racebookApi.Constants;
using racebookApi.Models;
using racebookApi.Models.DTOs.FromClient;
using racebookApi.Models.DTOs.ToClient;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services;

namespace racebookApiTests
{
    public class ModServiceTests
    {
        private ICloudinaryRepository _cloudinaryRepository;
        private IModRepository _modRepository;
        private IPreviewImageRepository _previewImageRepository;
        private IUserRepository _userRepository;
        private IFormFile _formFile;

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
            string modFileUrl = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf";

            for (int i = 0; i < 3; i++)
            {
                previewImages.Add(_formFile);
            }

            ModDto dto = new ModDto {
                Title = "custom skyboxes",
                Type = "Skybox",
                Description = "make sky colourful",
                ModFile = _formFile,
                PreviewImages = previewImages
                };

            _cloudinaryRepository.UploadAsync(dto.ModFile, FileType.Raw).Returns(modFileUrl);

            foreach (IFormFile previewImage in previewImages)
            {
                _cloudinaryRepository.UploadAsync(previewImage, FileType.Image).Returns("https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.jpg");
            }

            _modRepository.CreateMod(
                "9D51DE57-A958-4B74-B975-52A5F81C7F93",
                dto.Title,
                dto.Type,
                dto.Description,
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                DateOnly.FromDateTime(DateTime.Now).ToString(),
                modFileUrl
                ).Returns(Guid.NewGuid());

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.UploadMod(dto);

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
                modFileUrl
                );
        }

        [Test]
        public async Task GivenAModId_WhenDeleting_DiscardAllInformationAndFilesAssociatedWithIt()
        {
            //Arrange
            string modId = Guid.NewGuid().ToString();
            string placeholderModUrl = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf";
            string placeholderImageUrl = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/PreviewImages/mmk2a4zewxrop0ep9uur.jpg";
            List<string> imageUrls = new List<string>{
                placeholderImageUrl,
                placeholderImageUrl
            };

            _previewImageRepository.GetPreviewImageUrl(modId).Returns(imageUrls);
            _modRepository.GetModFileUrl(modId).Returns(placeholderModUrl);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.DeleteMod(modId);

            //Assert
            await _cloudinaryRepository.Received(3).DeleteAsync(Arg.Any<DeletionParams>());
            await _previewImageRepository.Received(1).DeletePreviewImageByModId(modId);
            await _modRepository.Received(1).DeleteMod(modId);
        }

        [Test]
        public async Task GivenModId_WhenGettingModDetails_ReturnAllInformationAssociatedWithTheId()
        {
            //Arrange
            Guid modId = Guid.NewGuid();
            Guid userID = Guid.NewGuid();

            Mod modInfo = new Mod
            {
                ModId = modId,
                Uid = userID,
                Title = "snow brighton",
                Type = "environment",
                Description = "yet another mod from sidali",
                FilePath = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf",
                EditDate = DateTime.Now,
                UploadDate = DateTime.Now,
            };

            GetModDto expectedResult =  new GetModDto
            {
                Id = modId.ToString(),
                Creator = "Sidali",
                Title = "snow brighton",
                Type = "environment",
                Description = "yet another mod from sidali",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
                ModFileUrl = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf",
                PreviewImageUrls = new List<string> { "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/PreviewImages/mmk2a4zewxrop0ep9uur.jpg" }
            };

            _modRepository.GetModById(modId.ToString()).Returns(modInfo);
            _userRepository.GetUsernameByUserId(userID.ToString()).Returns("Sidali");
            _previewImageRepository.GetPreviewImageUrl(modId.ToString()).Returns(new List<string> { "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/PreviewImages/mmk2a4zewxrop0ep9uur.jpg" });

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            GetModDto actualResult = await modService.GetMod(modId.ToString());

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
                Guid.NewGuid(),
                Guid.NewGuid(),
            };
            Guid userID = Guid.NewGuid();

            Mod modInfo = new Mod
            {
                ModId = modIds[0],
                Uid = userID,
                Title = "snow brighton",
                Type = "environment",
                Description = "yet another mod from sidali",
                FilePath = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf",
                EditDate = DateTime.Now,
                UploadDate = DateTime.Now,
            };

            GetModDto mod = new GetModDto
            {
                Id = modIds[0].ToString(),
                Creator = "Sidali",
                Title = "snow brighton",
                Type = "environment",
                Description = "yet another mod from sidali",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
                ModFileUrl = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf",
                PreviewImageUrls = new List<string> { "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/PreviewImages/mmk2a4zewxrop0ep9uur.jpg" }
            };

            List<GetModDto> expectedResult = new List<GetModDto>
            {
                mod,
                mod
            };

            _modRepository.GetAllModIds().Returns(modIds);

            foreach (Guid modId in modIds)
            {
                _modRepository.GetModById(modId.ToString()).Returns(modInfo);
                _userRepository.GetUsernameByUserId(userID.ToString()).Returns("Sidali");
                _previewImageRepository.GetPreviewImageUrl(modId.ToString()).Returns(new List<string> { "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/PreviewImages/mmk2a4zewxrop0ep9uur.jpg" });
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
                Guid.NewGuid(),
                Guid.NewGuid(),
            };
            Guid userID = Guid.NewGuid();

            Mod modInfo = new Mod
            {
                ModId = modIds[0],
                Uid = userID,
                Title = "snow brighton",
                Type = "environment",
                Description = "yet another mod from sidali",
                FilePath = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf",
                EditDate = DateTime.Now,
                UploadDate = DateTime.Now,
            };

            GetModDto mod = new GetModDto
            {
                Id = modIds[0].ToString(),
                Creator = "Sidali",
                Title = "snow brighton",
                Type = "environment",
                Description = "yet another mod from sidali",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
                ModFileUrl = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf",
                PreviewImageUrls = new List<string> { "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/PreviewImages/mmk2a4zewxrop0ep9uur.jpg" }
            };

            List<GetModDto> expectedResult = new List<GetModDto>{
                mod,
                mod
            };

            _modRepository.GetMyModIds(userID.ToString()).Returns(modIds);

            foreach (Guid modId in modIds)
            {
                _modRepository.GetModById(modId.ToString()).Returns(modInfo);
                _userRepository.GetUsernameByUserId(userID.ToString()).Returns("Sidali");
                _previewImageRepository.GetPreviewImageUrl(modId.ToString()).Returns(new List<string> { "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/PreviewImages/mmk2a4zewxrop0ep9uur.jpg" });
            }

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.GetMyMods(userID.ToString());

            //Assert
            await _modRepository.Received(2).GetModById(Arg.Any<string>());
            await _userRepository.Received(2).GetUsernameByUserId(Arg.Any<string>());
            await _previewImageRepository.Received(2).GetPreviewImageUrl(Arg.Any<string>());
        }

        [Test]
        public async Task GivenANotNullValueForTitle_WhenEditing_WriteTheChangesToDb()
        {
            //Arrange
            Guid modId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            ModEditDto modEdit = new ModEditDto
            {
                ModId = modId,
                Title = "edited Title",
            };

            Mod mod = new Mod
            {
                ModId = modId,
                Uid = userId,
                Title = "Title",
                Type = "Vehicle",
                Description = "is very cool",
                FilePath = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
            };

            _modRepository.GetModById(modId.ToString()).Returns(mod);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.EditMod(modEdit);

            //Assert
            await _modRepository.Received(1).EditMod(mod);
        }

        [Test]
        public async Task GivenANotNullValueForType_WhenEditing_WriteTheChangesToDb()
        {
            //Arrange
            Guid modId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            ModEditDto modEdit = new ModEditDto
            {
                ModId = modId,
                Type = "edited Type",
            };

            Mod mod = new Mod
            {
                ModId = modId,
                Uid = userId,
                Title = "Title",
                Type = "Vehicle",
                Description = "is very cool",
                FilePath = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
            };

            _modRepository.GetModById(modId.ToString()).Returns(mod);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.EditMod(modEdit);

            //Assert
            await _modRepository.Received(1).EditMod(mod);
        }

        [Test]
        public async Task GivenANotNullValueForDescription_WhenEditing_WriteTheChangesToDb()
        {
            //Arrange
            Guid modId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            ModEditDto modEdit = new ModEditDto
            {
                ModId = modId,
                Description = "edited description",
            };

            Mod mod = new Mod
            {
                ModId = modId,
                Uid = userId,
                Title = "Title",
                Type = "Vehicle",
                Description = "is very cool",
                FilePath = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
            };

            _modRepository.GetModById(modId.ToString()).Returns(mod);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.EditMod(modEdit);

            //Assert
            await _modRepository.Received(1).EditMod(mod);
        }

        [Test]
        public async Task GivenANotNullValueForModFile_WhenEditing_WriteTheChangesToDb()
        {
            //Arrange
            Guid modId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            string placeholderModUrl = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf";

            ModEditDto modEdit = new ModEditDto
            {
                ModId = modId,
                ModFile = _formFile,
            };

            Mod mod = new Mod
            {
                ModId = modId,
                Uid = userId,
                Title = "Title",
                Type = "Vehicle",
                Description = "is very cool",
                FilePath = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
            };

            _modRepository.GetModById(modId.ToString()).Returns(mod);
            _modRepository.GetModFileUrl(modId.ToString()).Returns(placeholderModUrl);
            _cloudinaryRepository.UploadAsync(modEdit.ModFile, FileType.Raw).Returns(placeholderModUrl);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.EditMod(modEdit);

            //Assert
            await _modRepository.Received(1).EditMod(mod);
        }

        [Test]
        public async Task GivenANotNullValueForImagesToBeDeleted_WhenEditing_WriteTheChangesToDb()
        {
            //Arrange
            Guid modId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            string placeholderImageUrl = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/PreviewImages/mmk2a4zewxrop0ep9uur.jpg";

            ModEditDto modEdit = new ModEditDto
            {
                ModId = modId,
                PreviewImagesToBeDeleted = new List<string> { placeholderImageUrl },
            };

            Mod mod = new Mod
            {
                ModId = modId,
                Uid = userId,
                Title = "Title",
                Type = "Vehicle",
                Description = "is very cool",
                FilePath = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
            };

            _modRepository.GetModById(modId.ToString()).Returns(mod);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.EditMod(modEdit);

            //Assert
            await _modRepository.Received(1).EditMod(mod);
            await _cloudinaryRepository.Received(1).DeleteAsync(Arg.Any<DeletionParams>());
            await _previewImageRepository.Received(1).DeletePreviewImageByUrl(Arg.Any<string>());
        }

        [Test]
        public async Task GivenANotNullValueForNewImages_WhenEditing_WriteTheChangesToDb()
        {
            //Arrange
            Guid modId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();

            ModEditDto modEdit = new ModEditDto
            {
                ModId = modId,
                NewPreviewImages = new List<IFormFile> { _formFile },
            };

            Mod mod = new Mod
            {
                ModId = modId,
                Uid = userId,
                Title = "Title",
                Type = "Vehicle",
                Description = "is very cool",
                FilePath = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf",
                UploadDate = DateTime.Now,
                EditDate = DateTime.Now,
            };

            _modRepository.GetModById(modId.ToString()).Returns(mod);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository, _userRepository);

            //Act
            await modService.EditMod(modEdit);

            //Assert
            await _modRepository.Received(1).EditMod(mod);
            await _cloudinaryRepository.Received(1).UploadAsync(Arg.Any<IFormFile>(), FileType.Image);
            await _previewImageRepository.Received(1).CreatePreviewImage(Arg.Any<Guid>(), Arg.Any<string>());
        }
    }
}