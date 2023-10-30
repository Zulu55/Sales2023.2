using Moq;
using Orders.Backend.Repositories;
using Orders.Backend.UnitsOfWork;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Tests.UnitsOfWork
{
    [TestClass]
    public class ProductsUnitOfWorkTests
    {
        private Mock<IGenericRepository<Product>> _repositoryMock = null!;
        private Mock<IProductsRepository> _productsRepositoryMock = null!;
        private ProductsUnitOfWork _unitOfWork = null!;

        [TestInitialize]
        public void SetUp()
        {
            _repositoryMock = new Mock<IGenericRepository<Product>>();
            _productsRepositoryMock = new Mock<IProductsRepository>();
            _unitOfWork = new ProductsUnitOfWork(_repositoryMock.Object, _productsRepositoryMock.Object);
        }

        [TestMethod]
        public async Task GetAsync_WithPagination_ReturnsProducts()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var expectedResponse = new Response<IEnumerable<Product>> { WasSuccess = true };
            _productsRepositoryMock.Setup(x => x.GetAsync(pagination))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _unitOfWork.GetAsync(pagination);

            // Assert
            Assert.AreEqual(expectedResponse, result);
            _productsRepositoryMock.Verify(x => x.GetAsync(pagination), Times.Once);
        }

        [TestMethod]
        public async Task GetTotalPagesAsync_ReturnsTotalPages()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var expectedResponse = new Response<int> { WasSuccess = true };
            _productsRepositoryMock.Setup(x => x.GetTotalPagesAsync(pagination))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _unitOfWork.GetTotalPagesAsync(pagination);

            // Assert
            Assert.AreEqual(expectedResponse, result);
            _productsRepositoryMock.Verify(x => x.GetTotalPagesAsync(pagination), Times.Once);
        }

        [TestMethod]
        public async Task GetAsync_ById_ReturnsProduct()
        {
            // Arrange
            var productId = 1;
            var expectedResponse = new Response<Product> { WasSuccess = true };
            _productsRepositoryMock.Setup(x => x.GetAsync(productId))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _unitOfWork.GetAsync(productId);

            // Assert
            Assert.AreEqual(expectedResponse, result);
            _productsRepositoryMock.Verify(x => x.GetAsync(productId), Times.Once);
        }

        [TestMethod]
        public async Task AddFullAsync_ReturnsProduct()
        {
            // Arrange
            var productDTO = new ProductDTO();
            var expectedResponse = new Response<Product> { WasSuccess = true };
            _productsRepositoryMock.Setup(x => x.AddFullAsync(productDTO))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _unitOfWork.AddFullAsync(productDTO);

            // Assert
            Assert.AreEqual(expectedResponse, result);
            _productsRepositoryMock.Verify(x => x.AddFullAsync(productDTO), Times.Once);
        }

        [TestMethod]
        public async Task UpdateFullAsync_ReturnsProduct()
        {
            // Arrange
            var productDTO = new ProductDTO();
            var expectedResponse = new Response<Product> { WasSuccess = true };
            _productsRepositoryMock.Setup(x => x.UpdateFullAsync(productDTO))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _unitOfWork.UpdateFullAsync(productDTO);

            // Assert
            Assert.AreEqual(expectedResponse, result);
            _productsRepositoryMock.Verify(x => x.UpdateFullAsync(productDTO), Times.Once);
        }

        [TestMethod]
        public async Task AddImageAsync_ReturnsImage()
        {
            // Arrange
            var imageDTO = new ImageDTO();
            var expectedResponse = new Response<ImageDTO> { WasSuccess = true };
            _productsRepositoryMock.Setup(x => x.AddImageAsync(imageDTO))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _unitOfWork.AddImageAsync(imageDTO);

            // Assert
            Assert.AreEqual(expectedResponse, result);
            _productsRepositoryMock.Verify(x => x.AddImageAsync(imageDTO), Times.Once);
        }

        [TestMethod]
        public async Task RemoveLastImageAsync_ReturnsImage()
        {
            // Arrange
            var imageDTO = new ImageDTO();
            var expectedResponse = new Response<ImageDTO> { WasSuccess = true };
            _productsRepositoryMock.Setup(x => x.RemoveLastImageAsync(imageDTO))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _unitOfWork.RemoveLastImageAsync(imageDTO);

            // Assert
            Assert.AreEqual(expectedResponse, result);
            _productsRepositoryMock.Verify(x => x.RemoveLastImageAsync(imageDTO), Times.Once);
        }
    }
}