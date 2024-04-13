using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using TangoMalena_BackEnd.Services.EmailService;

namespace AuthTest.Services;

public class IdentityEmailService(IOptions<EmailConfiguration> EmailConfiguration) : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailMessage = CreateEmailMessage(email, subject, htmlMessage);
        using (var client = new SmtpClient())
        {
            try
            {
                await client.ConnectAsync(EmailConfiguration.Value.SmtpServer, EmailConfiguration.Value.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(EmailConfiguration.Value.UserName, EmailConfiguration.Value.Password);
                await client.SendAsync(emailMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
    
    private MimeMessage CreateEmailMessage(string email, string subject, string htmlMessage)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Developer", EmailConfiguration.Value.From));
        emailMessage.To.Add(new MailboxAddress("Developer", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(TextFormat.Html) { Text = htmlMessage};
        return emailMessage;
    }
}