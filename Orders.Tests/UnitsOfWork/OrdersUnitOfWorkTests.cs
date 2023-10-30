﻿using Moq;
using Orders.Backend.Repositories;
using Orders.Backend.UnitsOfWork;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Tests.UnitsOfWork
{
    [TestClass]
    public class OrdersUnitOfWorkTests
    {
        private Mock<IGenericRepository<Order>> _mockGenericRepository = null!;
        private Mock<IOrdersRepository> _mockOrdersRepository = null!;
        private OrdersUnitOfWork _ordersUnitOfWork = null!;

        [TestInitialize]
        public void SetUp()
        {
            _mockGenericRepository = new Mock<IGenericRepository<Order>>();
            _mockOrdersRepository = new Mock<IOrdersRepository>();
            _ordersUnitOfWork = new OrdersUnitOfWork(_mockGenericRepository.Object, _mockOrdersRepository.Object);
        }

        [TestMethod]
        public async Task GetAsync_ShouldReturnOrders_WhenCalled()
        {
            // Arrange
            var email = "test@example.com";
            var paginationDTO = new PaginationDTO();
            var response = new Response<IEnumerable<Order>> { WasSuccess = true };
            _mockOrdersRepository.Setup(repo => repo.GetAsync(email, paginationDTO))
                .ReturnsAsync(response);

            // Act
            var result = await _ordersUnitOfWork.GetAsync(email, paginationDTO);

            // Assert
            Assert.AreEqual(response, result);
        }

        [TestMethod]
        public async Task GetTotalPagesAsync_ShouldReturnTotalPages_WhenCalled()
        {
            // Arrange
            var email = "test@example.com";
            var paginationDTO = new PaginationDTO();
            var response = new Response<int> { WasSuccess = true };
            _mockOrdersRepository.Setup(repo => repo.GetTotalPagesAsync(email, paginationDTO))
                .ReturnsAsync(response);

            // Act
            var result = await _ordersUnitOfWork.GetTotalPagesAsync(email, paginationDTO);

            // Assert
            Assert.AreEqual(response, result);
            _mockOrdersRepository.Verify(x => x.GetTotalPagesAsync(email, paginationDTO), Times.Once());
        }

        [TestMethod]
        public async Task GetAsync_WithId_ShouldReturnOrder_WhenCalled()
        {
            // Arrange
            var orderId = 1;
            var response = new Response<Order> { WasSuccess = true };
            _mockOrdersRepository.Setup(repo => repo.GetAsync(orderId))
                .ReturnsAsync(response);

            // Act
            var result = await _ordersUnitOfWork.GetAsync(orderId);

            // Assert
            Assert.AreEqual(response, result);
            _mockOrdersRepository.Verify(x => x.GetAsync(orderId), Times.Once());
        }

        [TestMethod]
        public async Task UpdateFullAsync_ShouldUpdateOrder_WhenCalled()
        {
            // Arrange
            var email = "test@example.com";
            var orderDTO = new OrderDTO();
            var response = new Response<Order> { WasSuccess = true };
            _mockOrdersRepository.Setup(repo => repo.UpdateFullAsync(email, orderDTO))
                .ReturnsAsync(response);

            // Act
            var result = await _ordersUnitOfWork.UpdateFullAsync(email, orderDTO);

            // Assert
            Assert.AreEqual(response, result);
            _mockOrdersRepository.Verify(x => x.UpdateFullAsync(email, orderDTO), Times.Once());
        }
    }
}