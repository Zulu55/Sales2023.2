﻿using Moq;
using Orders.Backend.Helpers;
using Orders.Backend.UnitsOfWork;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Tests.Helpers
{
    [TestClass]
    public class OrdersHelperTests
    {
        private Mock<IUserHelper> _userHelperMock = null!;
        private Mock<ITemporalOrdersUnitOfWork> _temporalOrdersUoWMock = null!;
        private Mock<IProductsUnitOfWork> _productsUoWMock = null!;
        private Mock<IOrdersUnitOfWork> _ordersUoWMock = null!;
        private OrdersHelper _ordersHelper = null!;

        [TestInitialize]
        public void Initialize()
        {
            _userHelperMock = new Mock<IUserHelper>();
            _temporalOrdersUoWMock = new Mock<ITemporalOrdersUnitOfWork>();
            _productsUoWMock = new Mock<IProductsUnitOfWork>();
            _ordersUoWMock = new Mock<IOrdersUnitOfWork>();
            _ordersHelper = new OrdersHelper(_userHelperMock.Object, _temporalOrdersUoWMock.Object, _productsUoWMock.Object, _ordersUoWMock.Object);
        }

        [TestMethod]
        public async Task ProcessOrderAsync_UserDoesNotExist_ReturnsFalseResponse()
        {
            // Arrange
            string email = "test@test.com";

            // Act
            var result = await _ordersHelper.ProcessOrderAsync(email, "remarks");

            // Assert
            Assert.IsFalse(result.WasSuccess);
            Assert.AreEqual("Usuario no válido", result.Message);
        }

        [TestMethod]
        public async Task ProcessOrderAsync_TemporalOrdersNotFound_ReturnsFalseResponse()
        {
            // Arrange
            string email = "test@test.com";
            var user = new User { Email = email };
            _userHelperMock.Setup(uh => uh.GetUserAsync(email)).ReturnsAsync(user);
            _temporalOrdersUoWMock.Setup(touw => touw.GetAsync(email))
                .ReturnsAsync(new Response<IEnumerable<TemporalOrder>> { WasSuccess = false });

            // Act
            var result = await _ordersHelper.ProcessOrderAsync(email, "remarks");

            // Assert
            Assert.IsFalse(result.WasSuccess);
            Assert.AreEqual("No hay detalle en la orden", result.Message);
        }

        [TestMethod]
        public async Task ProcessOrderAsync_InventoryCheckFails_ReturnsFalseResponse()
        {
            // Arrange
            string email = "test@test.com";
            var user = new User { Email = email };
            var temporalOrders = new List<TemporalOrder>
    {
        new TemporalOrder { Quantity = 5, Product = new Product { Id = 1, Name = "Product1", Stock = 3 } }
    };
            _userHelperMock.Setup(uh => uh.GetUserAsync(email)).ReturnsAsync(user);
            _temporalOrdersUoWMock.Setup(touw => touw.GetAsync(email))
                .ReturnsAsync(new Response<IEnumerable<TemporalOrder>> { WasSuccess = true, Result = temporalOrders });
            _productsUoWMock.Setup(puw => puw.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new Response<Product> { WasSuccess = true, Result = temporalOrders[0].Product });

            // Act
            var result = await _ordersHelper.ProcessOrderAsync(email, "remarks");

            // Assert
            Assert.IsFalse(result.WasSuccess);
            Assert.AreEqual($"Lo sentimos no tenemos existencias suficientes del producto {temporalOrders[0].Product!.Name}, para tomar su pedido. Por favor disminuir la cantidad o sustituirlo por otro.", result.Message);
        }

        [TestMethod]
        public async Task ProcessOrderAsync_HappyPath_ReturnsTrueResponse()
        {
            // Arrange
            string email = "test@test.com";
            var user = new User { Email = email };
            var temporalOrders = new List<TemporalOrder>
            {
                new TemporalOrder { Quantity = 2, Product = new Product { Id = 1, Name = "Product1", Stock = 5 }, Remarks = "Remarks1", Id = 1 }
            };
            _userHelperMock.Setup(uh => uh.GetUserAsync(email))
                .ReturnsAsync(user);
            _temporalOrdersUoWMock.Setup(touw => touw.GetAsync(email))
                .ReturnsAsync(new Response<IEnumerable<TemporalOrder>> { WasSuccess = true, Result = temporalOrders });
            _productsUoWMock.Setup(puw => puw.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new Response<Product> { WasSuccess = true, Result = temporalOrders[0].Product });
            _temporalOrdersUoWMock.Setup(touw => touw.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(new Response<TemporalOrder> { WasSuccess = true });
            _productsUoWMock.Setup(puw => puw.UpdateAsync(It.IsAny<Product>()))
                .ReturnsAsync(new Response<Product> { WasSuccess = true });
            _ordersUoWMock.Setup(ouw => ouw.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync(new Response<Order> { WasSuccess = true });

            // Act
            var result = await _ordersHelper.ProcessOrderAsync(email, "remarks");

            // Assert
            Assert.IsTrue(result.WasSuccess);
            _productsUoWMock.Verify(puw => puw.UpdateAsync(It.Is<Product>(p => p.Stock == 3)), Times.Once);
            _temporalOrdersUoWMock.Verify(touw => touw.DeleteAsync(1), Times.Once);
            _ordersUoWMock.Verify(ouw => ouw.AddAsync(It.Is<Order>(o => o.Remarks == "remarks")), Times.Once);
        }

        [TestMethod]
        public async Task ProcessOrderAsync_ProductNoAvailabe_ReturnsError()
        {
            // Arrange
            string email = "test@test.com";
            var user = new User { Email = email };
            var temporalOrders = new List<TemporalOrder>
            {
                new TemporalOrder { Quantity = 2, Product = new Product { Id = 1, Name = "Product1", Stock = 5 }, Remarks = "Remarks1", Id = 1 }
            };
            _userHelperMock.Setup(uh => uh.GetUserAsync(email))
                .ReturnsAsync(user);
            _temporalOrdersUoWMock.Setup(touw => touw.GetAsync(email))
                .ReturnsAsync(new Response<IEnumerable<TemporalOrder>> { WasSuccess = true, Result = temporalOrders });
            _productsUoWMock.Setup(puw => puw.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new Response<Product> { WasSuccess = false });
            _temporalOrdersUoWMock.Setup(touw => touw.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(new Response<TemporalOrder> { WasSuccess = true });
            _productsUoWMock.Setup(puw => puw.UpdateAsync(It.IsAny<Product>()))
                .ReturnsAsync(new Response<Product> { WasSuccess = true });
            _ordersUoWMock.Setup(ouw => ouw.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync(new Response<Order> { WasSuccess = true });

            // Act
            var result = await _ordersHelper.ProcessOrderAsync(email, "remarks");

            // Assert
            Assert.IsFalse(result.WasSuccess);
        }

        [TestMethod]
        public async Task ProcessOrderAsync_ProductNoAvailabeTwo_ReturnsError()
        {
            // Arrange
            string email = "test@test.com";
            var user = new User { Email = email };
            var temporalOrders = new List<TemporalOrder>
            {
                new TemporalOrder { Quantity = 2, Product = new Product { Id = 1, Name = "Product1", Stock = 5 }, Remarks = "Remarks1", Id = 1 }
            };
            _userHelperMock.Setup(uh => uh.GetUserAsync(email))
                .ReturnsAsync(user);
            _temporalOrdersUoWMock.Setup(touw => touw.GetAsync(email))
                .ReturnsAsync(new Response<IEnumerable<TemporalOrder>> { WasSuccess = true, Result = temporalOrders });
            _productsUoWMock.Setup(puw => puw.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new Response<Product> { WasSuccess = true });
            _temporalOrdersUoWMock.Setup(touw => touw.DeleteAsync(It.IsAny<int>()))
                .ReturnsAsync(new Response<TemporalOrder> { WasSuccess = true });
            _productsUoWMock.Setup(puw => puw.UpdateAsync(It.IsAny<Product>()))
                .ReturnsAsync(new Response<Product> { WasSuccess = true });
            _ordersUoWMock.Setup(ouw => ouw.AddAsync(It.IsAny<Order>()))
                .ReturnsAsync(new Response<Order> { WasSuccess = true });

            // Act
            var result = await _ordersHelper.ProcessOrderAsync(email, "remarks");

            // Assert
            Assert.IsFalse(result.WasSuccess);
        }
    }
}