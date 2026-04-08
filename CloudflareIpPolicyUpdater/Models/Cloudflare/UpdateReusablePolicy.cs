using System.Net;
using System.Text.Json.Serialization;

namespace CloudflareIpPolicyUpdater.Models.Cloudflare;

public class UpdateReusablePolicy
{
    public UpdateReusablePolicy(AccessReusablePolicy policy, IPAddress ipAddressV4, IPAddress? ipAddressV6 = null)
    {
        Name = policy.Name;
        Decision = policy.Decision.StringValue();

        Include = [];
        if (ipAddressV4 != IPAddress.None)
        {
            Include.Add(new { ip = new { ip = ipAddressV4.ToString() } });
        }
        if (ipAddressV6 is not null && ipAddressV6 != IPAddress.None)
        {
            Include.Add(new { ip = new { ip = ipAddressV6.ToString() } });
        }
    }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("decision")]
    public string Decision { get; set; } = string.Empty;

    [JsonPropertyName("include")]
    public List<object> Include { get; set; } = [];

    [JsonPropertyName("exclude")]
    public List<object>? Exclude { get; set; }

    [JsonPropertyName("require")]
    public List<object>? Require { get; set; }

    [JsonPropertyName("session_duration")]
    public string SessionDuration { get; set; } = string.Empty;

    [JsonPropertyName("isolation_required")]
    public bool? IsolationRequired { get; set; }

    [JsonPropertyName("purpose_justification_required")]
    public bool? PurposeJustificationRequired { get; set; }

    [JsonPropertyName("purpose_justification_prompt")]
    public string? PurposeJustificationPrompt { get; set; }
}
