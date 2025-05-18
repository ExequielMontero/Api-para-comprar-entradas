// Services/EmailService.cs
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
namespace Api_entradas.Services
{

    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody, byte[] attachment);
    }
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _cfg;
        public EmailService(IConfiguration cfg) => _cfg = cfg;

        public async Task SendEmailAsync(string to, string subject, string htmlBody, byte[] attachment)
        {
            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(_cfg["Mail:User"]));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlBody };
            builder.Attachments.Add("ticket.png", attachment, new ContentType("image", "png"));
            msg.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_cfg["Mail:SmtpServer"],
                                    int.Parse(_cfg["Mail:Port"]),
                                    SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_cfg["Mail:User"], _cfg["Mail:Password"]);
            await smtp.SendAsync(msg);
            await smtp.DisconnectAsync(true);
        }
    }
}
