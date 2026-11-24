using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit;

namespace ServiceManagerApp;

public class EmailSender
{
    private readonly string name;
    private readonly string email;
    private readonly string equipment;
    private readonly decimal cost;
    private readonly Status status;

    public EmailSender(string name, string email, string equipment, decimal cost, Status status)
    {
        this.name = name;
        this.email = email;
        this.equipment = equipment;
        this.cost = cost;
        this.status = status;
    }

    public async Task SendAsync()
    {
        var (subject, body) = status switch
        {
            Status.Diagnosing => (
                $"A(z) {equipment} nevű felszerelése átvizsgálásra került",
                $"A(z) {equipment} nevű felszerlése jelenleg átvizsgálás alatt van.\nA javítását rövid ídő belül elkezdjük"),
            Status.InRepair => (
                $"A(z) {equipment} nevű felszerelése javításra került",
                $"A(z) {equipment} nevű felszerlése jelenleg javítás alatt van.\nAz eszközét nem sokára átveheti"),
            Status.Finished => (
                $"A(z) {equipment} készen áll az átvételre",
                $"Tisztelt {name}!\nA(z) {equipment} eszközét átveheti üzletünk nyitvatartása szerint.\nFizetendő összeg: {cost} Ft"),
            Status.Cancelled => (
                $"A(z) {equipment} javítása eltörlésre került",
                $"Kérjük jöjjön be átvenni {equipment} nevű eszközét üzletünk nyitvatartása szerint"),
            _ => throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                "Email notifications are not supported for this service ticket status.")
        };

        var message = new MimeMessage();

        message.From.Add(new MailboxAddress("FixWare", "fixware8@gmail.com"));
        message.To.Add(new MailboxAddress(name, email));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = body
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