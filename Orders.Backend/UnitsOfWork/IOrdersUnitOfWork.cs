using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork
{
    public interface IOrdersUnitOfWork
    {
        Task<Response<IEnumerable<Order>>> GetAsync(string email, PaginationDTO pagination);

        Task<Response<int>> GetTotalPagesAsync(string email, PaginationDTO pagination);

        Task<Response<Order>> GetAsync(int id);

        Task<Response<Order>> UpdateFullAsync(string email, OrderDTO orderDTO);

        Task<Response<Order>> AddAsync(Order order);
    }
}