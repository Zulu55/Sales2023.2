using Orders.Shared.DTOs;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork
{
    public interface IGenericUnitOfWork<T> where T : class
    {
        Task<Response<IEnumerable<T>>> GetAsync(PaginationDTO pagination);

        Task<Response<int>> GetTotalPagesAsync(PaginationDTO pagination);

        Task<Response<T>> AddAsync(T model);

        Task<Response<T>> UpdateAsync(T model);

        Task<Response<T>> DeleteAsync(int id);

        Task<Response<T>> GetAsync(int id);
    }
}