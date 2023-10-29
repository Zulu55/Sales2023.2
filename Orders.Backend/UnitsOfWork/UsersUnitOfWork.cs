using Orders.Backend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork
{
    public class UsersUnitOfWork : IUsersUnitOfWork
    {
        private readonly IUsersRepository _usersRepository;

        public UsersUnitOfWork(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }

        public async Task<Response<User>> GetAsync(string email) => await _usersRepository.GetAsync(email);

        public async Task<Response<User>> GetAsync(Guid userId) => await _usersRepository.GetAsync(userId);

        public async Task<Response<IEnumerable<User>>> GetAsync(PaginationDTO pagination) => await _usersRepository.GetAsync(pagination);

        public async Task<Response<int>> GetTotalPagesAsync(PaginationDTO pagination) => await _usersRepository.GetTotalPagesAsync(pagination);
    }
}