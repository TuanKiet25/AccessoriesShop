using System.Threading.Tasks;

namespace AccessoriesShop.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string toName, string otpCode);
    }
}
