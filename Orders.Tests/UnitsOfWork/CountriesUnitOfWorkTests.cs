﻿using Moq;
using Orders.Backend.Repositories;
using Orders.Backend.UnitsOfWork;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Tests.UnitsOfWork
{
    [TestClass]
    public class CountriesUnitOfWorkTests
    {
        private Mock<IGenericRepository<Country>> _mockGenericRepository = null!;
        private Mock<ICountriesRepository> _mockCountriesRepository = null!;
        private CountriesUnitOfWork _unitOfWork = null!;

        [TestInitialize]
        public void Initialize()
        {
            _mockGenericRepository = new Mock<IGenericRepository<Country>>();
            _mockCountriesRepository = new Mock<ICountriesRepository>();
            _unitOfWork = new CountriesUnitOfWork(_mockGenericRepository.Object, _mockCountriesRepository.Object);
        }

        [TestMethod]
        public async Task GetAsync_WithPagination_ShouldReturnData()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var expectedResponse = new Response<IEnumerable<Country>> { WasSuccess = true };
            _mockCountriesRepository.Setup(repo => repo.GetAsync(pagination)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _unitOfWork.GetAsync(pagination);

            // Assert
            Assert.AreEqual(expectedResponse, result);
        }

        [TestMethod]
        public async Task GetTotalPagesAsync_ShouldReturnTotalPages()
        {
            // Arrange
            var pagination = new PaginationDTO();
            var expectedResponse = new Response<int> { WasSuccess = true };
            _mockCountriesRepository.Setup(repo => repo.GetTotalPagesAsync(pagination)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _unitOfWork.GetTotalPagesAsync(pagination);

            // Assert
            Assert.AreEqual(expectedResponse, result);
        }

        [TestMethod]
        public async Task GetAsync_WithId_ShouldReturnData()
        {
            // Arrange
            int id = 1;
            var expectedResponse = new Response<Country> { WasSuccess = true };
            _mockCountriesRepository.Setup(repo => repo.GetAsync(id)).ReturnsAsync(expectedResponse);

            // Act
            var result = await _unitOfWork.GetAsync(id);

            // Assert
            Assert.AreEqual(expectedResponse, result);
        }

        [TestMethod]
        public async Task GetComboAsync_ShouldReturnData()
        {
            // Arrange
            var expectedCountries = new List<Country> { new Country { Id = 1, Name = "Country1" } };
            _mockCountriesRepository.Setup(repo => repo.GetComboAsync()).ReturnsAsync(expectedCountries);

            // Act
            var result = await _unitOfWork.GetComboAsync();

            // Assert
            CollectionAssert.AreEqual(expectedCountries, new List<Country>(result));
        }
    }
}