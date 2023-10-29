using Orders.Backend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork
{
    public class OrdersUnitOfWork : GenericUnitOfWork<Order>, IOrdersUnitOfWork
    {
        private readonly IOrdersRepository _ordersRepository;

        public OrdersUnitOfWork(IGenericRepository<Order> repository, IOrdersRepository ordersRepository) : base(repository)
        {
            _ordersRepository = ordersRepository;
        }

        public async Task<Response<IEnumerable<Order>>> GetAsync(string email, PaginationDTO pagination) => await _ordersRepository.GetAsync(email, pagination);

        public async Task<Response<int>> GetTotalPagesAsync(string email, PaginationDTO pagination) => await _ordersRepository.GetTotalPagesAsync(email, pagination);

        public override async Task<Response<Order>> GetAsync(int id) => await _ordersRepository.GetAsync(id);

        public async Task<Response<Order>> UpdateFullAsync(string email, OrderDTO orderDTO) => await _ordersRepository.UpdateFullAsync(email, orderDTO);
    }
}