using Sales.Shared.Responses;

namespace Sales.Backend.Intertfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetAsync(int id);

        Task<IEnumerable<T>> GetAsync();

        Task<Response<T>> AddAsync(T entity);

        Task DeleteAsync(int id);

        Task<Response<T>> UpdateAsync(T entity);
    }
}
