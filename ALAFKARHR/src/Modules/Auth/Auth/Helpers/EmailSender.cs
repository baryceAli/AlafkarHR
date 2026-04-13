using Shared.Contracts.Messaging;
using System.Net;
using System.Net.Mail;

namespace Auth.Helpers;

public class EmailSender : IMessageSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        this._configuration = configuration;
    }
    public async Task SendAsync(string email, string subject, string htmlMessage)
    {

        var smtpHost = _configuration["Smtp:Host"];
        var smtpPort = int.Parse(_configuration?["Smtp:Port"] ?? "587");
        var smtpUser = _configuration["Smtp:Username"];
        var smtpPass = _configuration["Smtp:Password"];
        var fromEmail = _configuration["Smtp:From"];

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true,
            Timeout = 10000
        };

        using var mail = new MailMessage()
        {
            From = new MailAddress(fromEmail),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mail.To.Add(email);

        //return client.SendMailAsync(mail);
        try
        {
            await client.SendMailAsync(mail);
            //return res;
        }
        catch (SmtpException ex)
        {
            throw new Exception($"SMTP error: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to send email: {ex.Message}", ex);
        }
    }

    public Task SendAsync(string phoneNumber, string message)
    {
        throw new NotImplementedException();
    }
}
