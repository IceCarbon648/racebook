using Microsoft.AspNetCore.Http;
using NSubstitute;
using racebookApi.Constants;
using racebookApi.Repositories.Interfaces;
using racebookApi.Services;

namespace racebookApiTests
{
    public class ModServiceTests
    {
        private ICloudinaryRepository _cloudinaryRepository;
        private IModRepository _modRepository;
        private IPreviewImageRepository _previewImageRepository;
        private IFormFile _formFile;

        [SetUp]
        public void Setup()
        {
            _cloudinaryRepository = Substitute.For<ICloudinaryRepository>();
            _modRepository = Substitute.For<IModRepository>();
            _previewImageRepository = Substitute.For<IPreviewImageRepository>();
            _formFile = Substitute.For<IFormFile>();
        }

        [Test]
        public async Task GivenAModFile_WhenUploadingItToRepository_ReturnTheUrlForTheUploadedFile()
        {
            //Arrange
            string expectedResult = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf";
            _cloudinaryRepository.UploadAsync(_formFile, FileType.Raw).Returns(expectedResult);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository);

            //Act
            string actualResult = await modService.UploadModFile(_formFile);

            //Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task GivenPreviewImages_WhenUploadingItToRepository_ReturnTheUrlsForTheUploadedFiles()
        {
            //Arrange
            string placeholderUrl = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf";
            List<IFormFile> formFiles = new List<IFormFile>();
            List<string> expectedResult = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                formFiles.Add(_formFile);
            }

            for (int i = 0; i < 5; i++)
            {
                expectedResult.Add(placeholderUrl);
            }

            for (int i = 0; i < 5; i++)
            {
                _cloudinaryRepository.UploadAsync(formFiles[i], FileType.Image).Returns(expectedResult[i]);
            }

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository);

            //Act
            List<string> actualResult = await modService.UploadPreviewImages(formFiles);

            //Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task GivenModDetails_WhenSavingTheModDetailsToDatabase_ReturnTheIdentifierForTheMod()
        {
            //Arrange
            Guid expectedResult = Guid.NewGuid();

            string uid = Guid.NewGuid().ToString();
            string title = "Very Amaze";
            string type = "Vehicle";
            string description = "yet another skin from sidali";
            string todaysDate = DateOnly.FromDateTime(DateTime.Now).ToString();
            string modFileUrl = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf";

            _modRepository.CreateMod(uid, title, type, description, todaysDate, todaysDate, modFileUrl).Returns(expectedResult);

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository);

            //Act
            Guid actualResult = await modService.SaveModFile(uid, title, type, description, modFileUrl);

            //Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult));
        }

        [Test]
        public async Task GivenPreviewImageUrlsAndModIdentifier_WhenSavingImagesDetailsToDatabase_CallCreatePreviewImageForTheNumberOfReceivedImages()
        {
            //Arrange
            Guid modId = Guid.NewGuid();
            string placeholderUrl = "https://res.cloudinary.com/dt63xnsdx/raw/upload/v1779622210/Mods/mmk2a4zewxrop0ep9uur.tpf";
            List<string> previewImageUrls = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                previewImageUrls.Add(placeholderUrl);
            }

            ModService modService = new ModService(_cloudinaryRepository, _modRepository, _previewImageRepository);

            //Act
            await modService.SavePreviewImages(modId, previewImageUrls);

            //Assert
            await _previewImageRepository.Received(5).CreatePreviewImage(modId, placeholderUrl);
        }
    }
}