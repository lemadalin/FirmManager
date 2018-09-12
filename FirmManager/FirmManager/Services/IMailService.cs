using FirmManager.Models;

namespace FirmManager.Services
{
    public interface IMailService
    {
        void SendMail(Order order, Customer customer, string code);
    }
}