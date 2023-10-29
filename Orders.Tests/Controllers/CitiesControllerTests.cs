using Microsoft.AspNetCore.Mvc;
using Moq;
using Orders.Backend.Controllers;
using Orders.Backend.UnitsOfWork;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Tests.Controllers
{
    [TestClass]
    public class CitiesControllerTests
    {
        private Mock<IGenericUnitOfWork<City>> _mockGenericUnitOfWork = null!;
        private Mock<ICitiesUnitOfWork> _mockCitiesUnitOfWork = null!;
        private CitiesController _controller = null!;

        [TestInitialize]
        public void Initialize()
        {
            _mockGenericUnitOfWork = new Mock<IGenericUnitOfWork<City>>();
            _mockCitiesUnitOfWork = new Mock<ICitiesUnitOfWork>();
            _controller = new CitiesController(_mockGenericUnitOfWork.Object, _mockCitiesUnitOfWork.Object);
        }

        [TestMethod]
        public async Task GetComboAsync_ShouldReturnOkResult()
        {
            // Arrange
            var stateId = 1;
            var cities = new List<City> { new City { Id = 1, Name = "City1" }, new City { Id = 2, Name = "City2" } };
            _mockCitiesUnitOfWork.Setup(uow => uow.GetComboAsync(stateId)).ReturnsAsync(cities);

            // Act
            var result = await _controller.GetComboAsync(stateId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var resultValue = okResult.Value as IEnumerable<City>;
            Assert.IsNotNull(resultValue);
            Assert.AreEqual(2, resultValue.Count());
        }

        [TestMethod]
        public async Task GetAsync_ShouldReturnOkResult_WhenResponseIsSuccess()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var response = new Response<IEnumerable<City>> { WasSuccess = true, Result = new List<City>() };
            _mockCitiesUnitOfWork.Setup(uow => uow.GetAsync(pagination)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetAsync(pagination);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
        }

        [TestMethod]
        public async Task GetAsync_ShouldReturnBadRequest_WhenResponseIsNotSuccess()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var response = new Response<IEnumerable<City>> { WasSuccess = false };
            _mockCitiesUnitOfWork.Setup(uow => uow.GetAsync(pagination)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetAsync(pagination);

            // Assert
            var badRequestResult = result as BadRequestResult;
            Assert.IsNotNull(badRequestResult);
        }

        [TestMethod]
        public async Task GetPagesAsync_ShouldReturnOkResult_WhenResponseIsSuccess()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var response = new Response<int> { WasSuccess = true, Result = 1 };
            _mockCitiesUnitOfWork.Setup(uow => uow.GetTotalPagesAsync(pagination)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetPagesAsync(pagination);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(1, okResult.Value);
        }

        [TestMethod]
        public async Task GetPagesAsync_ShouldReturnBadRequest_WhenResponseIsNotSuccess()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var response = new Response<int> { WasSuccess = false };
            _mockCitiesUnitOfWork.Setup(uow => uow.GetTotalPagesAsync(pagination)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetPagesAsync(pagination);

            // Assert
            var badRequestResult = result as BadRequestResult;
            Assert.IsNotNull(badRequestResult);
        }
    }
}