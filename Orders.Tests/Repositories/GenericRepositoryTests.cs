using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;

namespace Orders.Tests.Repositories
{
    [TestClass]
    public class GenericRepositoryTests
    {
        private DataContext _context = null!;
        private GenericRepository<Category> _repository = null!;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            _repository = new GenericRepository<Category>(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task AddAsync_ShouldAddEntity()
        {
            var testEntity = new Category { Name = "Test" };
            var response = await _repository.AddAsync(testEntity);

            Assert.IsTrue(response.WasSuccess);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual("Test", response.Result.Name);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldDeleteEntity()
        {
            var testEntity = new Category { Name = "Test" };
            await _context.Set<Category>().AddAsync(testEntity);
            await _context.SaveChangesAsync();

            var response = await _repository.DeleteAsync(testEntity.Id);

            Assert.IsTrue(response.WasSuccess);
        }

        [TestMethod]
        public async Task DeleteAsync_EntityNotFound_ShouldReturnErrorResponse()
        {
            var response = await _repository.DeleteAsync(1);
            Assert.IsFalse(response.WasSuccess);
            Assert.AreEqual("Registro no encontrado", response.Message);
        }

        [TestMethod]
        public async Task GetAsync_ById_ShouldReturnEntity()
        {
            var testEntity = new Category { Name = "Test" };
            await _context.Set<Category>().AddAsync(testEntity);
            await _context.SaveChangesAsync();

            var response = await _repository.GetAsync(testEntity.Id);

            Assert.IsTrue(response.WasSuccess);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual("Test", response.Result.Name);
        }

        [TestMethod]
        public async Task GetAsync_ById_EntityNotFound_ShouldReturnErrorResponse()
        {
            var response = await _repository.GetAsync(1);
            Assert.IsFalse(response.WasSuccess);
            Assert.AreEqual("Registro no encontrado", response.Message);
        }

        [TestMethod]
        public async Task GetAsync_Pagination_ShouldReturnEntities()
        {
            await _context.Set<Category>().AddRangeAsync(new List<Category>
            {
                new Category { Name = "Test1" },
                new Category { Name = "Test2" },
                new Category { Name = "Test3" },
            });
            await _context.SaveChangesAsync();

            var paginationDTO = new PaginationDTO { RecordsNumber = 2 };
            var response = await _repository.GetAsync(paginationDTO);

            Assert.IsTrue(response.WasSuccess);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual(2, response.Result.Count());
        }

        [TestMethod]
        public async Task GetTotalPagesAsync_ShouldReturnTotalPages()
        {
            await _context.Set<Category>().AddRangeAsync(new List<Category>
            {
                new Category { Name = "Test1" },
                new Category { Name = "Test2" },
                new Category { Name = "Test3" },
            });
            await _context.SaveChangesAsync();

            var paginationDTO = new PaginationDTO { RecordsNumber = 2 };
            var response = await _repository.GetTotalPagesAsync(paginationDTO);

            Assert.IsTrue(response.WasSuccess);
            Assert.AreEqual(2, response.Result);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            var testEntity = new Category { Name = "Test" };
            await _context.Set<Category>().AddAsync(testEntity);
            await _context.SaveChangesAsync();

            testEntity.Name = "UpdatedTest";
            var response = await _repository.UpdateAsync(testEntity);

            Assert.IsTrue(response.WasSuccess);
            Assert.IsNotNull(response.Result);
            Assert.AreEqual("UpdatedTest", response.Result.Name);
        }
    }
}