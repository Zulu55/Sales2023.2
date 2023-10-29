using Orders.Shared.Responses;

namespace Orders.Backend.Helpers
{
    public interface IOrdersHelper
    {
        Task<Response<bool>> ProcessOrderAsync(string email, string remarks);
    }
}