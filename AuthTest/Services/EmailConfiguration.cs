namespace TangoMalena_BackEnd.Services.EmailService;

public class EmailConfiguration
{
    public List<string> Admins { get; set; }
    public string From { get; set; }
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}