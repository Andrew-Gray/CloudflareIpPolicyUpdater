using CloudflareIpPolicyUpdater.Models;
using CloudflareIpPolicyUpdater.Models.Config;
using CloudflareIpPolicyUpdater.Services;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json", optional: false);

IConfiguration config = builder.Build();

var cloudflareConfig = config.GetSection("Cloudflare").Get<CloudflareConfig>();
var gmailConfig = config.GetSection("Gmail").Get<GmailConfig>();
var generalConfig = config.GetSection("General").Get<GeneralConfig>();

try
{
    var fileService = new FileService(generalConfig.IpLogFile);
    var oldIpAddresses = await fileService.ReadPublicIpFromFile();

    var ipService = new IpService(generalConfig.NetworkAdapterName);
    var newIpAddresses = await ipService.GetIpsAsync();

    await fileService.WriteToFile(newIpAddresses);
    if (
        oldIpAddresses is not null
        && oldIpAddresses.PublicIp.Equals(newIpAddresses.PublicIp)
        && oldIpAddresses.LocalIp.Equals(newIpAddresses.LocalIp)
    )
    {
        Console.WriteLine("IP has not changed. No update needed.");
        return;
    }

    var emailService = new EmailService(gmailConfig.Email, gmailConfig.AppPassword);
    await emailService.SendEmail(oldIpAddresses ?? new IpAddresses(), newIpAddresses);

    if (oldIpAddresses is not null && oldIpAddresses.PublicIp.Equals(newIpAddresses.PublicIp))
    {
        Console.WriteLine("Public IP has not changed. No update needed.");
        return;
    }


    var cloudflareService = new CloudflareService(cloudflareConfig.ApiToken, cloudflareConfig.AccountId, cloudflareConfig.PolicyId);
    var policy = await cloudflareService.GetReusablePolicyAsync();

    if (policy is null)
    {
        Console.WriteLine($"No Policy Found");
        return;
    }

    bool updateSuccess = await cloudflareService.UpdateReusablePolicyAsync(newIpAddresses.PublicIp, policy);
    if (!updateSuccess)
    {
        Console.WriteLine("Failed to update Cloudflare policy.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}