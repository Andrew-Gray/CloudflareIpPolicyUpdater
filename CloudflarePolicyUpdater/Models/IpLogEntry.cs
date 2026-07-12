using System.Net;

namespace CloudflareIpPolicyUpdater.Models;

public class IpAddresses
{
    public IPAddress PublicIpV4 { get; set; } = IPAddress.None;
    public IPAddress PublicIpV6 { get; set; } = IPAddress.None;

    public IPAddress LocalIpV4 { get; set; } = IPAddress.None;
    public IPAddress LocalIpV6 { get; set; } = IPAddress.None;

    public void AddPublicIp(string ip)
    {
        PublicIpV4 = IPAddress.Parse(ip);
    }
    public void AddPublicIpV6(string ip)
    {
        PublicIpV6 = IPAddress.Parse(ip);
    }
    public void AddLocalIp(string ip)
    {
        LocalIpV4 = IPAddress.Parse(ip);
    }
    public void AddLocalIpV6(string ip)
    {
        LocalIpV6 = IPAddress.Parse(ip);
    }

    public bool Equals(IpAddresses other)
    {
        if (other is null) return false;
        return PublicIpEquals(other) && LocalIpEquals(other);
    }

    public bool PublicIpEquals(IpAddresses other)
    {
        if (other is null) return false;
        return PublicIpV4.Equals(other.PublicIpV4) && PublicIpV6.Equals(other.PublicIpV6);
    }

    public bool LocalIpEquals(IpAddresses other)
    {
        if (other is null) return false;
        return LocalIpV4.Equals(other.LocalIpV4) && LocalIpV6.Equals(other.LocalIpV6);
    }
}
