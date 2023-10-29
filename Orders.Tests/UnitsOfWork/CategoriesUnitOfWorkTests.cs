﻿using Moq;
using Orders.Backend.Repositories;
using Orders.Backend.UnitsOfWork;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Tests.UnitsOfWork
{
    [TestClass]
    public class CategoriesUnitOfWorkTests
    {
        private Mock<IGenericRepository<Category>> _mockGenericRepository = null!;
        private Mock<ICategoriesRepository> _mockCategoriesRepository = null!;
        private CategoriesUnitOfWork _unitOfWork = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockGenericRepository = new Mock<IGenericRepository<Category>>();
            _mockCategoriesRepository = new Mock<ICategoriesRepository>();
            _unitOfWork = new CategoriesUnitOfWork(_mockGenericRepository.Object, _mockCategoriesRepository.Object);
        }

        [TestMethod]
        public async Task GetAsync_CallsRepositoryAndReturnsResult()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var expectedResponse = new Response<IEnumerable<Category>> { Result = new List<Category>() };
            _mockCategoriesRepository.Setup(repo => repo.GetAsync(pagination)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _unitOfWork.GetAsync(pagination);

            // Assert
            Assert.AreEqual(expectedResponse, result);
            _mockCategoriesRepository.Verify(repo => repo.GetAsync(pagination), Times.Once);
        }

        [TestMethod]
        public async Task GetComboAsync_CallsRepositoryAndReturnsResult()
        {
            // Arrange
            var expectedCategories = new List<Category> { new Category() };
            _mockCategoriesRepository.Setup(repo => repo.GetComboAsync()).ReturnsAsync(expectedCategories);

            // Act
            var result = await _unitOfWork.GetComboAsync();

            // Assert
            Assert.AreEqual(expectedCategories, result);
            _mockCategoriesRepository.Verify(repo => repo.GetComboAsync(), Times.Once);
        }

        [TestMethod]
        public async Task GetTotalPagesAsync_CallsRepositoryAndReturnsResult()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var expectedResponse = new Response<int> { Result = 5 };
            _mockCategoriesRepository.Setup(repo => repo.GetTotalPagesAsync(pagination)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _unitOfWork.GetTotalPagesAsync(pagination);

            // Assert
            Assert.AreEqual(expectedResponse, result);
            _mockCategoriesRepository.Verify(repo => repo.GetTotalPagesAsync(pagination), Times.Once);
        }
    }
}