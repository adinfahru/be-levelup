using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LevelUp.API.Utilities;

public record EmailDto(
    string To,
    string Subject,
    string Body
);

public interface IEmailHandler
{
    Task EmailAsync(EmailDto emailDto);
}

public class EmailHandler : IEmailHandler
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _mailUsername;
    private readonly string _mailPassword;
    private readonly string _mailFrom;

    public EmailHandler(
        string smtpServer,
        int smtpPort,
        string mailUsername,
        string mailPassword,
        string mailFrom)
    {
        _smtpServer = smtpServer;
        _smtpPort = smtpPort;
        _mailUsername = mailUsername;
        _mailPassword = mailPassword;
        _mailFrom = mailFrom;
    }

    public async Task EmailAsync(EmailDto emailDto)
    {
        try
        {
            // Console.WriteLine("SMTP: preparing email");

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_mailFrom));
            email.To.Add(MailboxAddress.Parse(emailDto.To));
            email.Subject = emailDto.Subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailDto.Body
            };

            using var smtp = new SmtpClient();

            // Console.WriteLine($"SMTP: connecting {_smtpServer}:{_smtpPort}");
            await smtp.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.None);

            // Console.WriteLine("SMTP: sending email");
            await smtp.SendAsync(email);

        await smtp.DisconnectAsync(true);
        // Console.WriteLine("SMTP: email sent");
    }
    catch (Exception)
    {
        throw;
    }
}


}
