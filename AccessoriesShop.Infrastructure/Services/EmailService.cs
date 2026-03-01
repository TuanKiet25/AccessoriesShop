using AccessoriesShop.Application.Common.Settings;
using AccessoriesShop.Application.IServices;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly IConfiguration _configuration;
        public EmailService(IOptions<MailSettings> mailSettings, IConfiguration configuration)
        {
            _mailSettings = mailSettings.Value;
            _configuration = configuration;
        }

        public async Task SendOtpEmailAsync(string toEmail, string toName, string otpCode)
        {
            var host = _mailSettings.Host!;
            var port = _mailSettings.Port!;
            var email = _mailSettings.Email!;
            var password = _mailSettings.Password!;
            var displayname = _mailSettings.DisplayName!;

            //var host = _configuration["MailSettings:Host"]!;
            //var port = int.Parse(_configuration["MailSettings:Port"]!);
            //var email = _configuration["MailSettings:Email"]!;
            //var password = _configuration["MailSettings:Password"]!;
            //var displayname = _configuration["MailSettings:DisplayName"]!;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(displayname, email));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = "Xác thực tài khoản - Mã OTP của bạn";

            message.Body = new TextPart("html")
            {
                Text = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px;'>
                    <h2 style='color: #333;'>Xác thực tài khoản AccessoriesShop</h2>
                    <p>Xin chào <strong>{toName}</strong>,</p>
                    <p>Mã OTP của bạn là:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <span style='font-size: 36px; font-weight: bold; letter-spacing: 8px; color: #4CAF50; background: #f0f0f0; padding: 15px 30px; border-radius: 8px;'>{otpCode}</span>
                    </div>
                    <p>Mã này có hiệu lực trong <strong>20 phút</strong>.</p>
                    <p style='color: #888; font-size: 13px;'>Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua email này.</p>
                </div>"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(email, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
