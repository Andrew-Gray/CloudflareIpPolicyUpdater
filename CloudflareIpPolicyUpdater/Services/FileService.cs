using CloudflareIpPolicyUpdater.Models;
using System.Net;

namespace CloudflareIpPolicyUpdater.Services;

public class FileService(string ipLogFile)
{
    private readonly string _ipLogFile = ipLogFile;
    private const string _publicIpKey = "PublicIP";
    private const string _localIpKey = "LocalIP";


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
                case _publicIpKey:
                    entry.PublicIp = IPAddress.Parse(value);
                    break;
                case _localIpKey:
                    entry.LocalIp = IPAddress.Parse(value);
                    break;
            }
        }
        return entry;
    }

    public async Task WriteToFile(IpAddresses ipAddresses)
    {
        string logEntry = $"{_publicIpKey}:{ipAddresses.PublicIp}\n{_localIpKey}:{ipAddresses.LocalIp}";
        await File.WriteAllTextAsync(_ipLogFile, logEntry);
    }
}
