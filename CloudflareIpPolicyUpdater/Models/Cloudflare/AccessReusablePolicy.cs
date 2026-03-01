using System.Text.Json.Serialization;

namespace CloudflareIpPolicyUpdater.Models.Cloudflare;

public record class AccessReusablePolicy
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("decision")]
    public PolicyDecision Decision { get; init; }

    [JsonPropertyName("precedence")]
    public int Precedence { get; init; }

    [JsonPropertyName("include")]
    public List<object> Include { get; init; } = [];

    [JsonPropertyName("exclude")]
    public List<object> Exclude { get; init; } = [];

    [JsonPropertyName("require")]
    public List<object> Require { get; init; } = [];

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; init; }
}
