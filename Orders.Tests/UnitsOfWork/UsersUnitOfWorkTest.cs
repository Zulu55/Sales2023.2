using Moq;
using Orders.Backend.Repositories;
using Orders.Backend.UnitsOfWork;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Tests.UnitsOfWork
{
    [TestClass]
    public class UsersUnitOfWorkTest
    {
        private readonly Mock<IUsersRepository> _mockUsersRepository = new Mock<IUsersRepository>();
        private readonly UsersUnitOfWork _unitOfWork;

        public UsersUnitOfWorkTest()
        {
            _unitOfWork = new UsersUnitOfWork(_mockUsersRepository.Object);
        }

        [TestMethod]
        public async Task GetAsync_WithEmail_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var response = new Response<User> { WasSuccess = true };
            _mockUsersRepository.Setup(repo => repo.GetAsync(email)).ReturnsAsync(response);

            // Act
            var result = await _unitOfWork.GetAsync(email);

            // Assert
            Assert.AreEqual(response, result);
        }

        [TestMethod]
        public async Task GetAsync_WithUserId_ReturnsUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var response = new Response<User> { WasSuccess = true };
            _mockUsersRepository.Setup(repo => repo.GetAsync(userId))
                .ReturnsAsync(response);

            // Act
            var result = await _unitOfWork.GetAsync(userId);

            // Assert
            Assert.AreEqual(response, result);
        }

        [TestMethod]
        public async Task GetAsync_WithPagination_ReturnsUsers()
        {
            // Arrange
            var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
            var response = new Response<IEnumerable<User>> { WasSuccess = true };
            _mockUsersRepository.Setup(repo => repo.GetAsync(pagination))
                .ReturnsAsync(response);

            // Act
            var result = await _unitOfWork.GetAsync(pagination);

            // Assert
            Assert.AreEqual(response, result);
        }

        [TestMethod]
        public async Task GetTotalPagesAsync_WithPagination_ReturnsTotalPages()
        {
            // Arrange
            var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10 };
            var response = new Response<int> { WasSuccess = true, Result = 5 };
            _mockUsersRepository.Setup(repo => repo.GetTotalPagesAsync(pagination))
                .ReturnsAsync(response);

            // Act
            var result = await _unitOfWork.GetTotalPagesAsync(pagination);

            // Assert
            Assert.AreEqual(response, result);
        }
    }
}