namespace CloudflareIpPolicyUpdater.Models.Config;

public record class CloudflareConfig
{
    public string ApiToken { get; init; } = string.Empty;
    public string AccountId { get; init; } = string.Empty;
    public string PolicyId { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public int? Ipv6PrefixLength { get; init; } = null;
}
