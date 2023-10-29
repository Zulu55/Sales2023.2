using Orders.Shared.Responses;

namespace Orders.Backend.Helpers
{
    namespace Orders.Backend.Helpers
    {
        public interface IMailHelper
        {
            Response<string> SendMail(string toName, string toEmail, string subject, string body);
        }
    }
}