using CloudflareIpPolicyUpdater.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Text;

namespace CloudflareIpPolicyUpdater.Services;

public class EmailService(string gmailEmail, string gmailAppPassword)
{
    private readonly string _gmailEmail = gmailEmail;
    private readonly string _gmailAppPassword = gmailAppPassword;

    public async Task SendEmail(IpAddresses oldIpAddresses, IpAddresses newIpAddresses)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Andrew Gray", _gmailEmail));
        message.To.Add(new MailboxAddress("Andrew Gray", _gmailEmail));
        message.Subject = "IP Address Update!";

        message.Body = new TextPart("html")
        {
            Text = FormatIpChangeMessage(oldIpAddresses, newIpAddresses)
        };

        using var client = new SmtpClient();
        client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
        client.Authenticate(_gmailEmail, _gmailAppPassword);

        client.Send(message);
        client.Disconnect(true);
    }

    private static string FormatIpChangeMessage(IpAddresses oldIpAddresses, IpAddresses newIpAddresses)
    {
        var updatedRowStyles = "style='background-color: #d24b4b; color: #fff; font-weight: bold;'";
        var isLocalIpChanged = !oldIpAddresses.LocalIp.Equals(newIpAddresses.LocalIp) ? updatedRowStyles : "";
        var isPublicIpChanged = !oldIpAddresses.PublicIp.Equals(newIpAddresses.PublicIp) ? updatedRowStyles : "";

        var sb = new StringBuilder();

        sb.Append("<h2 style='color: #2c3e50; font-family: sans-serif;'>IP Address has been updated</h2>");
        sb.Append("<table style='width: 100%; border-collapse: collapse; font-family: sans-serif;'>");
        sb.Append("<thead>");
        sb.Append("<tr style='background-color: #f2f2f2; text-align: left;'>");
        sb.Append("<th style='padding: 10px; border: 1px solid #ddd;'></th>");
        sb.Append("<th style='padding: 10px; border: 1px solid #ddd;'>Old</th>");
        sb.Append("<th style='padding: 10px; border: 1px solid #ddd;'>New</th>");
        sb.Append("</tr>");
        sb.Append("</thead>");
        sb.Append("<tbody>");
        sb.Append($"<tr {isLocalIpChanged}>");
        sb.Append("<td style='padding: 10px; border: 1px solid #ddd;'>Local</td>");
        sb.Append($"<td style='padding: 10px; border: 1px solid #ddd;'>{oldIpAddresses.LocalIp}</td>");
        sb.Append($"<td style='padding: 10px; border: 1px solid #ddd;'>{newIpAddresses.LocalIp}</td>");
        sb.Append("</tr>");
        sb.Append($"<tr {isPublicIpChanged}>");
        sb.Append("<td style='padding: 10px; border: 1px solid #ddd;'>Public</td>");
        sb.Append($"<td style='padding: 10px; border: 1px solid #ddd;'>{oldIpAddresses.PublicIp}</td>");
        sb.Append($"<td style='padding: 10px; border: 1px solid #ddd;'>{newIpAddresses.PublicIp}</td>");
        sb.Append("</tr>");
        sb.Append("</tbody>");
        sb.Append("</table>");

        return sb.ToString();
    }
}
