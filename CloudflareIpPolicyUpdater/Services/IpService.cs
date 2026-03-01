using CloudflareIpPolicyUpdater.Models;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace CloudflareIpPolicyUpdater.Services;

public class IpService(string? networkAdapterName = null)
{
    private static readonly HttpClient _client = new();
    private readonly string _ipifyUrl = "https://api.ipify.org";
    private readonly string? _networkAdapterName = networkAdapterName;

    public async Task<IpAddresses> GetIpsAsync()
    {
        var ipAddresses = new IpAddresses();
        ipAddresses.addPublicIp(await GetPublicIpAsync());
        ipAddresses.addLocalIp(GetLocalIPAddress());

        return ipAddresses;
    }

    private async Task<string> GetPublicIpAsync()
    {
        var response = await _client.GetStringAsync(_ipifyUrl);
        return response.Trim();
    }

    private string GetLocalIPAddress()
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
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.Address.ToString();
                }
            }
        }

        return "No IPv4 address found";
    }
}
