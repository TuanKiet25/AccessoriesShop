using System.Threading.Tasks;

namespace AccessoriesShop.Application.IServices
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string toName, string otpCode);
    }
}
