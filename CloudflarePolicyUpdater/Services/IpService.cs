using CloudflareIpPolicyUpdater.Models;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace CloudflareIpPolicyUpdater.Services;

public class IpService(string? networkAdapterName = null)
{
    private static readonly HttpClient _client = new();
    private readonly string _ipifyUrlV4 = "https://api.ipify.org";
    private readonly string _ipifyUrlV6 = "https://api6.ipify.org";
    private readonly string? _networkAdapterName = networkAdapterName;

    public async Task<IpAddresses> GetIpsAsync()
    {
        var ipAddresses = new IpAddresses();
        ipAddresses.AddPublicIp(await GetPublicIpAsync(_ipifyUrlV4));

        var publicIpV6 = await GetPublicIpAsync(_ipifyUrlV6);
        if (!string.IsNullOrEmpty(publicIpV6) && publicIpV6 != "::1")
        {
            ipAddresses.AddPublicIpV6(publicIpV6);
        }

        ipAddresses.AddLocalIp(GetLocalIPAddress(AddressFamily.InterNetwork));

        var localIpV6 = GetLocalIPAddress(AddressFamily.InterNetworkV6);
        if (!string.IsNullOrEmpty(localIpV6))
        {
            ipAddresses.AddLocalIpV6(localIpV6);
        }

        return ipAddresses;
    }

    private async Task<string> GetPublicIpAsync(string url)
    {
        try
        {
            var response = await _client.GetStringAsync(url);
            return response.Trim();
        }
        catch
        {
            return string.Empty;
        }
    }

    private string GetLocalIPAddress(AddressFamily addressFamily)
    {
        var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (var ni in networkInterfaces)
        {
            // Filter by adapter name if specified
            if (_networkAdapterName is not null && ni.Name != _networkAdapterName)
                continue;

            // Skip disconnected or disabled adapters
            if (ni.OperationalStatus != OperationalStatus.Up)
                continue;

            var ipProperties = ni.GetIPProperties();

            foreach (var ip in ipProperties.UnicastAddresses)
            {
                if (ip.Address.AddressFamily == addressFamily)
                {
                    // Skip link-local IPv6 addresses (fe80::)
                    if (addressFamily == AddressFamily.InterNetworkV6 && ip.Address.IsIPv6LinkLocal)
                        continue;

                    return ip.Address.ToString();
                }
            }
        }

        return "No address found";
    }

    public static string FormatIpString(IPAddress ip)
    {
        return ip == IPAddress.None ? "" : ip.ToString();
    }
}
