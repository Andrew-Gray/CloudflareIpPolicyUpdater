using CloudflareIpPolicyUpdater.Models;
using System.Net;

namespace CloudflareIpPolicyUpdater.Services;

public class FileService(string ipLogFile)
{
    private readonly string _ipLogFile = ipLogFile;
    private const string _publicIpv4Key = "PublicIPv4";
    private const string _localIpv4Key = "LocalV4IPv4";
    private const string _publicIpv6Key = "PublicIPv6";
    private const string _localIpv6Key = "LocalIPv6";


    public async Task<IpAddresses?> ReadPublicIpFromFile()
    {
        if (!File.Exists(_ipLogFile))
        {
            return null;
        }

        var entry = new IpAddresses();
        var lines = File.ReadAllLines(_ipLogFile);

        foreach (var line in lines)
        {
            var parts = line.Split(':', 2);
            if (parts.Length < 2) continue;

            string key = parts[0].Trim();
            string value = parts[1].Trim();

            switch (key)
            {
                case _publicIpv4Key:
                    entry.PublicIpV4 = IPAddress.Parse(value);
                    break;
                case _localIpv4Key:
                    entry.LocalIpV4 = IPAddress.Parse(value);
                    break;
                case _publicIpv6Key:
                    entry.PublicIpV6 = IPAddress.Parse(value);
                    break;
                case _localIpv6Key:
                    entry.LocalIpV6 = IPAddress.Parse(value);
                    break;
            }
        }
        return entry;
    }

    public async Task WriteToFile(IpAddresses ipAddresses)
    {
        string logEntry = $"{_publicIpv4Key}:{ipAddresses.PublicIpV4}\n{_localIpv4Key}:{ipAddresses.LocalIpV4}\n{_publicIpv6Key}:{ipAddresses.PublicIpV6}\n{_localIpv6Key}:{ipAddresses.LocalIpV6}";
        await File.WriteAllTextAsync(_ipLogFile, logEntry);
    }
}
