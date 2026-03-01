namespace CloudflareIpPolicyUpdater.Models.Config;

public record class GmailConfig
{
    public string Email { get; init; } = string.Empty;
    public string AppPassword { get; init; } = string.Empty;
}
