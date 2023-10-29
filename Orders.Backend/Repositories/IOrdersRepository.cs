using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories
{
    public interface IOrdersRepository
    {
        Task<Response<IEnumerable<Order>>> GetAsync(string email, PaginationDTO pagination);

        Task<Response<int>> GetTotalPagesAsync(string email, PaginationDTO pagination);

        Task<Response<Order>> GetAsync(int id);

        Task<Response<Order>> UpdateFullAsync(string email, OrderDTO orderDTO);
    }
}