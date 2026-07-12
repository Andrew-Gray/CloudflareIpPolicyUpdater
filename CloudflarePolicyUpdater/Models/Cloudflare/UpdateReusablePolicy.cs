using System.Net;
using System.Text.Json.Serialization;

namespace CloudflareIpPolicyUpdater.Models.Cloudflare;

public class UpdateReusablePolicy
{
    public UpdateReusablePolicy(AccessReusablePolicy policy, IPAddress ipAddressV4, IPAddress? ipAddressV6 = null, int? ipv6PrefixLength = null, string? country = null)
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
            string ipStr = ipAddressV6.ToString();
            if (ipv6PrefixLength.HasValue)
            {
                ipStr = GetIpv6NetworkPrefix(ipAddressV6, ipv6PrefixLength.Value);
            }
            Include.Add(new { ip = new { ip = ipStr } });
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            Require = [new { geo = new { country_code = country } }];
        }
    }

    private static string GetIpv6NetworkPrefix(IPAddress address, int prefixLength)
    {
        if (address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            return address.ToString();
        }
        if (prefixLength < 0 || prefixLength > 128)
        {
            return address.ToString();
        }
        var bytes = address.GetAddressBytes();
        int fullBytes = prefixLength / 8;
        int remBits = prefixLength % 8;
        if (fullBytes < bytes.Length)
        {
            if (remBits > 0)
            {
                int mask = 0xFF << (8 - remBits);
                bytes[fullBytes] = (byte)(bytes[fullBytes] & mask);
                for (int i = fullBytes + 1; i < bytes.Length; i++) bytes[i] = 0;
            }
            else
            {
                for (int i = fullBytes; i < bytes.Length; i++) bytes[i] = 0;
            }
        }
        var network = new IPAddress(bytes);
        return $"{network}/{prefixLength}";
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
