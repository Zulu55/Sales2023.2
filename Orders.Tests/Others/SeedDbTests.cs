using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using Moq;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Backend.Services;
using Orders.Shared.Responses;

namespace Orders.Tests.Others
{
    [TestClass]
    public class SeedDbTests
    {
        private SeedDb _seedDb = null!;
        private Mock<IApiService> _apiServiceMock = null!;
        private Mock<IUserHelper> _userHelperMock = null!;
        private Mock<IFileStorage> _fileStorageMock = null!;
        private Mock<IRuntimeInformationWrapper> _runtimeInformationMock = null!;
        private DataContext _context = null!;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "OrdersDbTest")
                .Options;
            _context = new DataContext(options);

            _apiServiceMock = new Mock<IApiService>();
            _userHelperMock = new Mock<IUserHelper>();
            _fileStorageMock = new Mock<IFileStorage>();
            _runtimeInformationMock = new Mock<IRuntimeInformationWrapper>();

            _seedDb = new SeedDb(_context, _apiServiceMock.Object, _userHelperMock.Object, _fileStorageMock.Object, _runtimeInformationMock.Object);
        }

        [TestMethod]
        public async Task SeedAsync_WithNoAPiCountriesResponseAndWindowsOS_ShouldSeedData()
        {
            // Arrange
            _runtimeInformationMock.Setup(r => r.IsOSPlatform(OSPlatform.Windows))
                .Returns(true);
            _fileStorageMock.Setup(x => x.SaveFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("imageUrl");
            _apiServiceMock.Setup(x => x.GetAsync<List<CountryResponse>>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new Response<List<CountryResponse>> { WasSuccess = false });

            // Act
            await _seedDb.SeedAsync();

            // Assert
            Assert.IsTrue(await _context.Countries.AnyAsync());
            Assert.IsTrue(await _context.Categories.AnyAsync());
            Assert.IsTrue(await _context.Products.AnyAsync());
            Assert.IsTrue(await _context.ProductCategories.AnyAsync());
            Assert.IsTrue(await _context.ProductImages.AnyAsync());
        }

        [TestMethod]
        public async Task SeedAsync_WithAPiCountriesResponseAndWindowsOS_ShouldSeedData()
        {
            // Arrange
            _runtimeInformationMock.Setup(r => r.IsOSPlatform(OSPlatform.Windows))
                .Returns(false);
            _fileStorageMock.Setup(x => x.SaveFileAsync(It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("imageUrl");

            var countryResponse = new Response<List<CountryResponse>>
            {
                WasSuccess = true,
                Result = new List<CountryResponse>
                {
                    new CountryResponse { Id = 1, Name = "Some", Iso2 = "SO" }
                }
            };
            _apiServiceMock.Setup(x => x.GetAsync<List<CountryResponse>>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(countryResponse);

            var stateResponse = new Response<List<StateResponse>>
            {
                WasSuccess = true,
                Result = new List<StateResponse>
                {
                    new StateResponse { Id = 1, Name = "Some", Iso2 = "SO" }
                }
            };
            _apiServiceMock.Setup(x => x.GetAsync<List<StateResponse>>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(stateResponse);

            var cityResponse = new Response<List<CityResponse>>
            {
                WasSuccess = true,
                Result = new List<CityResponse>
                {
                    new CityResponse { Id = 1, Name = "Some" },
                    new CityResponse { Id = 2, Name = "Mosfellsbær" },
                    new CityResponse { Id = 3, Name = "Șăulița" }
                }
            };
            _apiServiceMock.Setup(x => x.GetAsync<List<CityResponse>>(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(cityResponse);

            // Act
            await _seedDb.SeedAsync();

            // Assert
            Assert.IsTrue(await _context.Countries.AnyAsync());
            Assert.IsTrue(await _context.Categories.AnyAsync());
            Assert.IsTrue(await _context.Products.AnyAsync());
            Assert.IsTrue(await _context.ProductCategories.AnyAsync());
            Assert.IsTrue(await _context.ProductImages.AnyAsync());
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}