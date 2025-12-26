namespace ClinikTime.service.Email;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
public class EmailService : IEmailService
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _senderEmail;
    private readonly string _password;

    public EmailService(IConfiguration config)
    {
        var section = config.GetSection("EmailSettings");
        _host = section["Host"];
        _port = int.Parse(section["Port"]);
        _senderEmail = section["SenderEmail"];
        _password = section["Password"];
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var client = new SmtpClient(_host, _port)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(_senderEmail, _password)
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_senderEmail),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(to);

        await client.SendMailAsync(mailMessage);
    }
}