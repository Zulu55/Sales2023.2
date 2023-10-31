using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;

namespace Orders.Tests.Repositories
{
    [TestClass]
    public class UsersRepositoryTest
    {
        private DataContext _context = null!;
        private UsersRepository _repository = null!;

        private readonly Guid _guid = Guid.NewGuid();

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "OrdersDatabase")
                .Options;

            _context = new DataContext(options);
            _repository = new UsersRepository(_context);

            // Seed the database with test data
            var country = new Country
            {
                Name = "Country",
                States = new List<State>
                {
                    new State
                    {
                        Name = "State",
                        Cities = new List<City>
                        {
                            new City { Name = "City" }
                        }
                    }
                }
            };
            _context.Countries.Add(country);
            _context.SaveChanges();

            var user1 = new User { Id = "1", FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Address = "Some", Document = "Any", CityId = 1 };
            var user2 = new User { Id = _guid.ToString(), FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com", Address = "Some", Document = "Any", CityId = 1 };
            _context.Users.AddRange(user1, user2);
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAsync_WithEmail_UserExists_ReturnsUser()
        {
            // Arrange
            var email = "john.doe@example.com";

            // Act
            var result = await _repository.GetAsync(email);

            // Assert
            Assert.IsTrue(result.WasSuccess);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual("John", result.Result.FirstName);
        }

        [TestMethod]
        public async Task GetAsync_WithEmail_UserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var email = "nonexistent@example.com";

            // Act
            var result = await _repository.GetAsync(email);

            // Assert
            Assert.IsFalse(result.WasSuccess);
            Assert.IsNull(result.Result);
            Assert.AreEqual("Usuario no encontrado", result.Message);
        }

        [TestMethod]
        public async Task GetAsync_WithUserId_UserExists_ReturnsUser()
        {
            // Act
            var result = await _repository.GetAsync(_guid);

            // Assert
            Assert.IsTrue(result.WasSuccess);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual("Jane", result.Result.FirstName);
        }

        [TestMethod]
        public async Task GetAsync_WithUserId_UserDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var result = await _repository.GetAsync(userId);

            // Assert
            Assert.IsFalse(result.WasSuccess);
            Assert.IsNull(result.Result);
            Assert.AreEqual("Usuario no encontrado", result.Message);
        }

        [TestMethod]
        public async Task GetAsync_WithPagination_ReturnsUsers()
        {
            // Arrange
            var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10, Filter = "J" };

            // Act
            var result = await _repository.GetAsync(pagination);

            // Assert
            Assert.IsTrue(result.WasSuccess);
            Assert.IsNotNull(result.Result);
            Assert.AreEqual(2, result.Result.Count());
        }

        [TestMethod]
        public async Task GetTotalPagesAsync_WithPagination_ReturnsTotalPages()
        {
            // Arrange
            var pagination = new PaginationDTO { Page = 1, RecordsNumber = 1, Filter = "J" };

            // Act
            var result = await _repository.GetTotalPagesAsync(pagination);

            // Assert
            Assert.IsTrue(result.WasSuccess);
            Assert.AreEqual(2, result.Result);
        }

        [TestMethod]
        public async Task GetTotalPagesAsync_WithFilter_ReturnsFilteredTotalPages()
        {
            // Arrange
            var pagination = new PaginationDTO { Page = 1, RecordsNumber = 10, Filter = "John" };

            // Act
            var result = await _repository.GetTotalPagesAsync(pagination);

            // Assert
            Assert.IsTrue(result.WasSuccess);
            Assert.AreEqual(1, result.Result);
        }
    }
}