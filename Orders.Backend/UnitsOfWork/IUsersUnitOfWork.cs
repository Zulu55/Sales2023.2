using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork
{
    public interface IUsersUnitOfWork
    {
        Task<Response<User>> GetAsync(string email);

        Task<Response<User>> GetAsync(Guid userId);

        Task<Response<IEnumerable<User>>> GetAsync(PaginationDTO pagination);

        Task<Response<int>> GetTotalPagesAsync(PaginationDTO pagination);
    }
}