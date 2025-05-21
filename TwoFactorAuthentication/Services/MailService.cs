using System.Net.Mail;
using System.Net;


namespace TwoFactorAuthentication.Services
{
    public class MailService
    {
        public void SendEmail(string fromEmail, string toEmail, string subject, string body, string smtpServer, int port, string username, string password)
        {
            var message = new MailMessage();
            message.From = new MailAddress(fromEmail);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true; // HTML body gönderiyorsan true yap

            var smtpClient = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true // Güvenlik için SSL/TLS açmak önemli
            };

            smtpClient.Send(message);
        }
    }
}



