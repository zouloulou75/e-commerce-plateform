// Services/EmailService.cs (version amélioré avec beau template)
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace GadgetStore.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = configuration["EmailSettings:Username"] ?? throw new ArgumentNullException("EmailSettings:Username");
            _smtpPassword = configuration["EmailSettings:Password"] ?? throw new ArgumentNullException("EmailSettings:Password");
            _fromEmail = configuration["EmailSettings:FromEmail"] ?? _smtpUsername;
            _fromName = configuration["EmailSettings:FromName"] ?? "Tekupstore";
        }

        public async Task SendOrderConfirmationAsync(string toEmail, string customerName, int orderId, decimal totalAmount)
        {
            var subject = $"Order Confirmed! Thank you for shopping with Tekupstore #{orderId}";

            var body = $@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Order Confirmation</title>
                <link href=""https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700&display=swap"" rel=""stylesheet"">
            </head>
            <body style=""margin:0; padding:0; background:#f4f7fc; font-family:'Inter', Arial, sans-serif;"">
                <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" style=""background:#f4f7fc; padding:20px;"">
                    <tr>
                        <td align=""center"">
                            <!-- Main Container -->
                            <table role=""presentation"" width=""600"" cellspacing=""0"" cellpadding=""0"" style=""background:#ffffff; border-radius:16px; overflow:hidden; box-shadow:0 10px 30px rgba(0,0,0,0.08);"">
                                <!-- Header -->
                                <tr>
                                    <td style=""background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding:40px 30px; text-align:center;"">
                                        <h1 style=""color:white; margin:0; font-size:28px; font-weight:700;"">TekUpStore</h1>
                                        <p style=""color:#e0dfff; margin:10px 0 0; font-size:16px;"">Thank you for your order!</p>
                                    </td>
                                </tr>

                                <!-- Success Icon -->
                                <tr>
                                    <td align=""center"" style=""padding:30px 0 20px;"">
                                        <div style=""width:80px; height:80px; background:#d4edda; border-radius:50%; display:inline-flex; align-items:center; justify-content:center;"">
                                            <span style=""font-size:48px;"">Check</span>
                                        </div>
                                        <h2 style=""margin:20px 0 10px; color:#28a745; font-size:26px;"">Order Confirmed!</h2>
                                        <p style=""color:#555; font-size:17px; margin:0;"">Hi <strong>{customerName}</strong>,</p>
                                        <p style=""color:#555; font-size:17px; margin:10px 0 0;"">Your order has been successfully placed and is being processed.</p>
                                    </td>
                                </tr>

                                <!-- Order Details Card -->
                                <tr>
                                    <td style=""padding:0 40px 40px;"">
                                        <div style=""background:#f8f9ff; border-radius:12px; padding:25px; text-align:center; margin:20px 0;"">
                                            <p style=""margin:0 0 12px; color:#666; font-size:15px;"">Order Number</p>
                                            <h3 style=""margin:0; color:#4e73df; font-size:32px; font-weight:700; letter-spacing:2px;"">#{orderId}</h3>
                                        </div>

                                        <table role=""presentation"" width=""100%"" style=""margin:30px 0;"">
                                            <tr>
                                                <td style=""padding:12px 0; border-bottom:1px solid #eee;"">
                                                    <span style=""color:#666; font-size:15px;"">Order Date</span><br>
                                                    <strong style=""font-size:16px;"">{DateTime.Now:dddd, MMMM dd, yyyy 'at' h:mm tt}</strong>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding:12px 0; border-bottom:1px solid #eee;"">
                                                    <span style=""color:#666; font-size:15px;"">Total Amount</span><br>
                                                    <strong style=""font-size:24px; color:#28a745;"">${totalAmount:N2}</strong>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style=""padding:12px 0;"">
                                                    <span style=""color:#666; font-size:15px;"">Payment Method</span><br>
                                                    <strong style=""font-size:16px;"">Paid via Card / COD</strong>
                                                </td>
                                            </tr>
                                        </table>
                                      
                                    </td>
                                </tr>

                                <!-- Footer -->
                                <tr>
                                    <td style=""background:#1a2c5; padding:30px; text-align:center;"">
                                        <p style=""color:white; margin:0; font-size:14px;"">
                                            © {DateTime.Now.Year} <strong>TekupStore</strong>. All rights reserved.<br>
                                            <a href=""#"" style=""color:#e0dfff; text-decoration:underline;"">Unsubscribe</a> • 
                                            <a href=""#"" style=""color:#e0dfff; text-decoration:underline;"">Privacy Policy</a>
                                        </p>
                                    </td>
                                </tr>
                            </table>

                            <!-- Credit: Designed with love by your dev
                        </td>
                    </tr>
                </table>
            </body>
            </html>";

            await SendEmailAsync(toEmail, subject, body);
        }

        private async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using var client = new SmtpClient(_smtpServer, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);
            await client.SendMailAsync(mailMessage);
        }
    }
}