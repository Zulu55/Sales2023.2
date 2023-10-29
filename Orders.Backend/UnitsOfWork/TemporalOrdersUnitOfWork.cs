using Orders.Backend.Repositories;
using Orders.Shared.DTOs;
using Orders.Shared.Entites;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork
{
    public class TemporalOrdersUnitOfWork : GenericUnitOfWork<TemporalOrder>, ITemporalOrdersUnitOfWork
    {
        private readonly ITemporalOrdersRepository _temporalOrdersRepository;

        public TemporalOrdersUnitOfWork(IGenericRepository<TemporalOrder> repository, ITemporalOrdersRepository temporalOrdersRepository) : base(repository)
        {
            _temporalOrdersRepository = temporalOrdersRepository;
        }

        public async Task<Response<TemporalOrderDTO>> AddFullAsync(string email, TemporalOrderDTO temporalOrderDTO) => await _temporalOrdersRepository.AddFullAsync(email, temporalOrderDTO);

        public async Task<Response<IEnumerable<TemporalOrder>>> GetAsync(string email) => await _temporalOrdersRepository.GetAsync(email);

        public async Task<Response<int>> GetCountAsync(string email) => await _temporalOrdersRepository.GetCountAsync(email);

        public async Task<Response<TemporalOrder>> PutFullAsync(TemporalOrderDTO temporalOrderDTO) => await _temporalOrdersRepository.PutFullAsync(temporalOrderDTO);

        public override async Task<Response<TemporalOrder>> GetAsync(int id) => await _temporalOrdersRepository.GetAsync(id);
    }
}