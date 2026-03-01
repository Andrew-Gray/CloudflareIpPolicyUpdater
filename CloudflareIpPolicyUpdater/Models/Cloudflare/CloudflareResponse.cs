using System.Text.Json.Serialization;

namespace CloudflareIpPolicyUpdater.Models.Cloudflare;

public record class CloudflareResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("errors")]
    public List<CloudflareError> Errors { get; init; } = [];

    [JsonPropertyName("result")]
    public T Result { get; init; } = default!;
}