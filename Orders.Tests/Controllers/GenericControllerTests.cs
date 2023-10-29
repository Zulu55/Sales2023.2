﻿using Microsoft.AspNetCore.Mvc;
using Moq;
using Orders.Backend.Controllers;
using Orders.Backend.UnitsOfWork;
using Orders.Shared.DTOs;
using Orders.Shared.Responses;

namespace Orders.Tests.Controllers
{
    [TestClass]
    public class GenericControllerTests
    {
        private Mock<IGenericUnitOfWork<object>> _mockUnitOfWork = null!;
        private PaginationDTO _paginationDTO = null!;
        private object _testModel = null!;
        private int _testId;

        [TestInitialize]
        public void Setup()
        {
            _mockUnitOfWork = new Mock<IGenericUnitOfWork<object>>();
            _paginationDTO = new PaginationDTO();
            _testModel = new object();
            _testId = 1;
        }

        [TestMethod]
        public async Task GetAsync_Pagination_Success()
        {
            // Arrange
            var response = new Response<IEnumerable<object>> { WasSuccess = true };
            _mockUnitOfWork.Setup(u => u.GetAsync(It.IsAny<PaginationDTO>()))
                .ReturnsAsync(response);

            var controller = new GenericController<object>(_mockUnitOfWork.Object);

            // Act
            var result = await controller.GetAsync(_paginationDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetAsync_Pagination_Failure()
        {
            // Arrange
            var response = new Response<IEnumerable<object>> { WasSuccess = false };
            _mockUnitOfWork.Setup(u => u.GetAsync(It.IsAny<PaginationDTO>()))
                .ReturnsAsync(response);

            var controller = new GenericController<object>(_mockUnitOfWork.Object);

            // Act
            var result = await controller.GetAsync(_paginationDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task GetPagesAsync_Success()
        {
            // Arrange
            var response = new Response<int> { WasSuccess = true, Result = 5 };
            _mockUnitOfWork.Setup(u => u.GetTotalPagesAsync(It.IsAny<PaginationDTO>()))
                    .ReturnsAsync(response);

            var controller = new GenericController<object>(_mockUnitOfWork.Object);

            // Act
            var result = await controller.GetPagesAsync(_paginationDTO);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(5, okResult.Value);
        }

        [TestMethod]
        public async Task GetPagesAsync_Failure()
        {
            // Arrange
            var response = new Response<int> { WasSuccess = false };
            _mockUnitOfWork.Setup(u => u.GetTotalPagesAsync(It.IsAny<PaginationDTO>()))
                            .ReturnsAsync(response);

            var controller = new GenericController<object>(_mockUnitOfWork.Object);

            // Act
            var result = await controller.GetPagesAsync(_paginationDTO);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestResult));
        }

        [TestMethod]
        public async Task GetAsync_Id_Success()
        {
            // Arrange
            var response = new Response<object> { WasSuccess = true, Result = _testModel };
            _mockUnitOfWork.Setup(u => u.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(response);

            var controller = new GenericController<object>(_mockUnitOfWork.Object);

            // Act
            var result = await controller.GetAsync(_testId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(_testModel, okResult.Value);
        }

        [TestMethod]
        public async Task GetAsync_Id_NotFound()
        {
            // Arrange
            var response = new Response<object> { WasSuccess = false };
            _mockUnitOfWork.Setup(u => u.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(response);

            var controller = new GenericController<object>(_mockUnitOfWork.Object);

            // Act
            var result = await controller.GetAsync(_testId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task PostAsync_Success()
        {
            // Arrange
            var response = new Response<object> { WasSuccess = true, Result = _testModel };
            _mockUnitOfWork.Setup(u => u.AddAsync(It.IsAny<object>()))
                .ReturnsAsync(response);

            var controller = new GenericController<object>(_mockUnitOfWork.Object);

            // Act
            var result = await controller.PostAsync(_testModel);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(_testModel, okResult.Value);
        }

        [TestMethod]
        public async Task PostAsync_Failure()
        {
            // Arrange
            var response = new Response<object> { WasSuccess = false, Message = "Error" };
            _mockUnitOfWork.Setup(u => u.AddAsync(It.IsAny<object>()))
                .ReturnsAsync(response);

            var controller = new GenericController<object>(_mockUnitOfWork.Object);

            // Act
            var result = await controller.PostAsync(_testModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Error", badRequestResult.Value);
        }

        [TestMethod]
        public async Task PutAsync_Success()
        {
            // Arrange
            var response = new Response<object> { WasSuccess = true, Result = _testModel };
            _mockUnitOfWork.Setup(u => u.UpdateAsync(It.IsAny<object>()))
                .ReturnsAsync(response);

            var controller = new GenericController<object>(_mockUnitOfWork.Object);

            // Act
            var result = await controller.PutAsync(_testModel);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(_testModel, okResult.Value);
        }

        [TestMethod]
        public async Task PutAsync_Failure()
        {
            // Arrange
            var response = new Response<object> { WasSuccess = false, Message = "Error" };
            _mockUnitOfWork.Setup(u => u.UpdateAsync(It.IsAny<object>()))
                .ReturnsAsync(response);

            var controller = new GenericController<object>(_mockUnitOfWork.Object);

            // Act
            var result = await controller.PutAsync(_testModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Error", badRequestResult.Value);
        }

        [TestMethod]
        public async Task DeleteAsync_Success()
        {
            // Arrange
            var response = new Response<object> { WasSuccess = true, Result = _testModel };
            _mockUnitOfWork.Setup(u => u.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(response);
            _mockUnitOfWork.Setup(u => u.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(response);

            var controller = new GenericController<object>(_mockUnitOfWork.Object);

            // Act
            var result = await controller.DeleteAsync(_testId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteAsync_GetFailed()
        {
            // Arrange
            var response = new Response<object> { WasSuccess = false };
            _mockUnitOfWork.Setup(u => u.GetAsync(It.IsAny<int>()))
                            .ReturnsAsync(response);

            var controller = new GenericController<object>(_mockUnitOfWork.Object);

            // Act
            var result = await controller.DeleteAsync(_testId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task DeleteAsync_DeleteFailed()
        {
            // Arrange
            var responseTrue = new Response<object> { WasSuccess = true, Result = _testModel };
            var responseFalse = new Response<object> { WasSuccess = false, Message = "Error" };
            _mockUnitOfWork.Setup(u => u.GetAsync(It.IsAny<int>()))
                            .ReturnsAsync(responseTrue);
            _mockUnitOfWork.Setup(u => u.DeleteAsync(It.IsAny<int>()))
                            .ReturnsAsync(responseFalse);

            var controller = new GenericController<object>(_mockUnitOfWork.Object);

            // Act
            var result = await controller.DeleteAsync(_testId);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Error", badRequestResult.Value);
        }
    }
}