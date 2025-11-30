// Services/IEmailService.cs
using System.Threading.Tasks;

namespace GadgetStore.Services
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(string toEmail, string customerName, int orderId, decimal totalAmount);
    }
}