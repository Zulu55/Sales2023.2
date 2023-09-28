using Sales.Shared.Responses;

namespace Sales.Backend.Services
{
    public interface IApiService
    {
        Task<Response<T>> GetAsync<T>(string servicePrefix, string controller);
    }
}
