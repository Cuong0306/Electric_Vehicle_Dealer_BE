using ElectricVehicleDealer.BLL.Intergations.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var smtpConfig = _config.GetSection("EmailSettings");

        using var smtp = new SmtpClient
        {
            Host = smtpConfig["Host"],
            Port = int.Parse(smtpConfig["Port"]),
            EnableSsl = bool.Parse(smtpConfig["EnableSsl"]),
            Credentials = new NetworkCredential(
                smtpConfig["Username"],
                smtpConfig["Password"]
            )
        };

        var mail = new MailMessage
        {
            From = new MailAddress(smtpConfig["FromEmail"], "EV Dealer"),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        await smtp.SendMailAsync(mail);
    }


    public async Task SendEmailWithAttachmentAsync(
    string to,
    string subject,
    string htmlBody,
    byte[] attachmentBytes,
    string attachmentName
)
    {
        var smtpConfig = _config.GetSection("EmailSettings");

        var enableSslStr = smtpConfig["EnableSsl"];
        bool enableSsl = true; 
        if (!string.IsNullOrEmpty(enableSslStr))
        {
            bool.TryParse(enableSslStr, out enableSsl);
        }

        int port = 587;
        if (int.TryParse(smtpConfig["Port"], out int parsedPort))
        {
            port = parsedPort;
        }

        using var smtp = new SmtpClient
        {
            Host = smtpConfig["Host"],
            Port = port,
            EnableSsl = enableSsl,
            Credentials = new NetworkCredential(
                smtpConfig["Username"],
                smtpConfig["Password"]
            )
        };

        var mail = new MailMessage
        {
            From = new MailAddress(smtpConfig["FromEmail"], "EV Dealer"),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };

        mail.To.Add(to);

        if (attachmentBytes != null && attachmentBytes.Length > 0)
        {
            var attachment = new Attachment(new MemoryStream(attachmentBytes), attachmentName);
            mail.Attachments.Add(attachment);
        }

        await smtp.SendMailAsync(mail);
    }
}
