namespace CloudflareIpPolicyUpdater.Models.Config;

public record class GeneralConfig
{
    public string NetworkAdapterName { get; init; } = string.Empty;
    public string IpLogFile { get; init; } = string.Empty;
}
