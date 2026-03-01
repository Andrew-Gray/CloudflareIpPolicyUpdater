using System.Net;

namespace CloudflareIpPolicyUpdater.Models;

public class IpAddresses
{
    public IPAddress PublicIp { get; set; } = IPAddress.None;
    public IPAddress LocalIp { get; set; } = IPAddress.None;

    public void addPublicIp(string ip)
    {
        PublicIp = IPAddress.Parse(ip);
    }
    public void addLocalIp(string ip)
    {
        LocalIp = IPAddress.Parse(ip);
    }
}
