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
    public class ProductsControllerTests
    {
        private Mock<IGenericUnitOfWork<Product>> _unitOfWorkMock = null!;
        private Mock<IProductsUnitOfWork> _productsUnitOfWorkMock = null!;
        private ProductsController _controller = null!;

        [TestInitialize]
        public void Initialize()
        {
            _unitOfWorkMock = new Mock<IGenericUnitOfWork<Product>>();
            _productsUnitOfWorkMock = new Mock<IProductsUnitOfWork>();
            _controller = new ProductsController(_unitOfWorkMock.Object, _productsUnitOfWorkMock.Object);
        }

        [TestMethod]
        public async Task GetAsync_NoSuccess_ReturnsError()
        {
            // Arrange
            var pagination = new PaginationDTO();
            _productsUnitOfWorkMock.Setup(x => x.GetAsync(pagination))
                .ReturnsAsync(new Response<IEnumerable<Product>>() { WasSuccess = false });

            // Act
            var result = await _controller.GetAsync(pagination);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
            _productsUnitOfWorkMock.Verify(x => x.GetAsync(pagination), Times.Once());
        }

        [TestMethod]
        public async Task GetAsync_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var pagination = new PaginationDTO();
            _productsUnitOfWorkMock.Setup(x => x.GetAsync(pagination))
                .ReturnsAsync(new Response<IEnumerable<Product>>() { WasSuccess = true });

            // Act
            var result = await _controller.GetAsync(pagination);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _productsUnitOfWorkMock.Verify(x => x.GetAsync(pagination), Times.Once());
        }

        [TestMethod]
        public async Task GetPagesAsync_WhenCalled_ReturnsOkResult()
        {
            // Arrange
            var pagination = new PaginationDTO();
            _productsUnitOfWorkMock.Setup(x => x.GetTotalPagesAsync(pagination))
                .ReturnsAsync(new Response<int>() { WasSuccess = true, Result = 5 });

            // Act
            var result = await _controller.GetPagesAsync(pagination);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _productsUnitOfWorkMock.Verify(x => x.GetTotalPagesAsync(pagination), Times.Once());
        }

        [TestMethod]
        public async Task GetPagesAsync_WhenFailed_ReturnsBadRequest()
        {
            // Arrange
            var pagination = new PaginationDTO();
            _productsUnitOfWorkMock.Setup(x => x.GetTotalPagesAsync(pagination))
                .ReturnsAsync(new Response<int>() { WasSuccess = false });

            // Act
            var result = await _controller.GetPagesAsync(pagination);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
            _productsUnitOfWorkMock.Verify(x => x.GetTotalPagesAsync(pagination), Times.Once());
        }

        [TestMethod]
        public async Task GetAsync_ById_WhenFound_ReturnsOkResult()
        {
            // Arrange
            int productId = 1;
            _productsUnitOfWorkMock.Setup(x => x.GetAsync(productId))
                .ReturnsAsync(new Response<Product>() { WasSuccess = true });

            // Act
            var result = await _controller.GetAsync(productId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _productsUnitOfWorkMock.Verify(x => x.GetAsync(productId), Times.Once());
        }

        [TestMethod]
        public async Task GetAsync_ById_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            int productId = 1;
            _productsUnitOfWorkMock.Setup(x => x.GetAsync(productId))
                .ReturnsAsync(new Response<Product>() { WasSuccess = false, Message = "Not Found" });

            // Act
            var result = await _controller.GetAsync(productId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            _productsUnitOfWorkMock.Verify(x => x.GetAsync(productId), Times.Once());
        }

        [TestMethod]
        public async Task PostFullAsync_WhenAdded_ReturnsOkResult()
        {
            // Arrange
            var productDTO = new ProductDTO();
            _productsUnitOfWorkMock.Setup(x => x.AddFullAsync(productDTO))
                .ReturnsAsync(new Response<Product>() { WasSuccess = true });

            // Act
            var result = await _controller.PostFullAsync(productDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _productsUnitOfWorkMock.Verify(x => x.AddFullAsync(productDTO), Times.Once());
        }

        [TestMethod]
        public async Task PostFullAsync_WhenFailed_ReturnsNotFound()
        {
            // Arrange
            var productDTO = new ProductDTO();
            _productsUnitOfWorkMock.Setup(x => x.AddFullAsync(productDTO))
                .ReturnsAsync(new Response<Product>() { WasSuccess = false, Message = "Not Found" });

            // Act
            var result = await _controller.PostFullAsync(productDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            _productsUnitOfWorkMock.Verify(x => x.AddFullAsync(productDTO), Times.Once());
        }

        [TestMethod]
        public async Task PutFullAsync_WhenUpdated_ReturnsOkResult()
        {
            // Arrange
            var productDTO = new ProductDTO();
            _productsUnitOfWorkMock.Setup(x => x.UpdateFullAsync(productDTO))
                .ReturnsAsync(new Response<Product>() { WasSuccess = true });

            // Act
            var result = await _controller.PutFullAsync(productDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _productsUnitOfWorkMock.Verify(x => x.UpdateFullAsync(productDTO), Times.Once());
        }

        [TestMethod]
        public async Task PutFullAsync_WhenFailed_ReturnsNotFound()
        {
            // Arrange
            var productDTO = new ProductDTO();
            _productsUnitOfWorkMock.Setup(x => x.UpdateFullAsync(productDTO))
                .ReturnsAsync(new Response<Product>() { WasSuccess = false, Message = "Not Found" });

            // Act
            var result = await _controller.PutFullAsync(productDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
            _productsUnitOfWorkMock.Verify(x => x.UpdateFullAsync(productDTO), Times.Once());
        }

        [TestMethod]
        public async Task PostAddImagesAsync_WhenSuccess_ReturnsOkResult()
        {
            // Arrange
            var imageDTO = new ImageDTO();
            _productsUnitOfWorkMock.Setup(x => x.AddImageAsync(imageDTO))
                .ReturnsAsync(new Response<ImageDTO>() { WasSuccess = true });

            // Act
            var result = await _controller.PostAddImagesAsync(imageDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _productsUnitOfWorkMock.Verify(x => x.AddImageAsync(imageDTO), Times.Once());
        }

        [TestMethod]
        public async Task PostAddImagesAsync_WhenFailed_ReturnsBadRequest()
        {
            // Arrange
            var imageDTO = new ImageDTO();
            _productsUnitOfWorkMock.Setup(x => x.AddImageAsync(imageDTO))
                .ReturnsAsync(new Response<ImageDTO>() { WasSuccess = false, Message = "Failed to add image" });

            // Act
            var result = await _controller.PostAddImagesAsync(imageDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _productsUnitOfWorkMock.Verify(x => x.AddImageAsync(imageDTO), Times.Once());
        }

        [TestMethod]
        public async Task PostRemoveLastImageAsync_WhenSuccess_ReturnsOkResult()
        {
            // Arrange
            var imageDTO = new ImageDTO();
            _productsUnitOfWorkMock.Setup(x => x.RemoveLastImageAsync(imageDTO))
                .ReturnsAsync(new Response<ImageDTO>() { WasSuccess = true });

            // Act
            var result = await _controller.PostRemoveLastImageAsync(imageDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _productsUnitOfWorkMock.Verify(x => x.RemoveLastImageAsync(imageDTO), Times.Once());
        }

        [TestMethod]
        public async Task PostRemoveLastImageAsync_WhenFailed_ReturnsBadRequest()
        {
            // Arrange
            var imageDTO = new ImageDTO();
            _productsUnitOfWorkMock.Setup(x => x.RemoveLastImageAsync(imageDTO))
                .ReturnsAsync(new Response<ImageDTO>() { WasSuccess = false, Message = "Failed to remove image" });

            // Act
            var result = await _controller.PostRemoveLastImageAsync(imageDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _productsUnitOfWorkMock.Verify(x => x.RemoveLastImageAsync(imageDTO), Times.Once());
        }
    }
}