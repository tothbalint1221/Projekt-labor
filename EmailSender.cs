using System.Configuration;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace ServiceManagerApp;

public class EmailSender
{
    private readonly string name;
    private readonly string email;
    private readonly string equipment;
    private readonly decimal cost;

    public EmailSender(string name, string email, string equipment, decimal cost)
    {
        this.name = name;
        this.email = email;
        this.equipment = equipment;
        this.cost = cost;
    }

    public async Task SendAsync()
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("FixWare", "fixware8@gmail.com"));
        message.To.Add(new MailboxAddress(name,email));
        message.Subject = $"A(z) {equipment} készen áll az átvételre";

        message.Body = new TextPart("plain")
        {
            Text = $"Tisztelt {name}!\nA(z) {equipment} eszközét átveheti üzletünk nyitvatartása szerint.\nFizetendő összeg: {cost} Ft"
        };

        using var client = new SmtpClient();

        IConfiguration configuration = new ConfigurationBuilder().AddUserSecrets<App>().Build();

        string? appPassword = configuration["Smtp:Password"];

        if (string.IsNullOrWhiteSpace(appPassword))
        {
            throw new InvalidOperationException("Missing Smtp:Password in User Secrets.");
        }

        await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync("fixware8@gmail.com", appPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}