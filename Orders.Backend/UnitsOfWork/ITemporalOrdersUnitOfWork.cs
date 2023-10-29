using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork
{
    public interface ITemporalOrdersUnitOfWork
    {
        Task<Response<TemporalOrderDTO>> AddFullAsync(string email, TemporalOrderDTO temporalOrderDTO);

        Task<Response<IEnumerable<TemporalOrder>>> GetAsync(string email);

        Task<Response<int>> GetCountAsync(string email);

        Task<Response<TemporalOrder>> GetAsync(int id);

        Task<Response<TemporalOrder>> PutFullAsync(TemporalOrderDTO temporalOrderDTO);

        Task<Response<TemporalOrder>> DeleteAsync(int id);
    }
}