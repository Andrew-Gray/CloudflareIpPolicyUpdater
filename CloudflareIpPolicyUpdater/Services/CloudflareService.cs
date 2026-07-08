using CloudflareIpPolicyUpdater.Models.Cloudflare;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CloudflareIpPolicyUpdater.Services;

public class CloudflareService(string apiToken, string accountId, string policyId)
{
    private static readonly HttpClient _client = new();
    private readonly string _cloudflareBaseUrl = "https://api.cloudflare.com/client/v4";
    private readonly string _apiToken = apiToken;
    private readonly string _accountId = accountId;
    private readonly string _policyId = policyId;


    public async Task<AccessReusablePolicy?> GetReusablePolicyAsync()
    {
        var url = $"{_cloudflareBaseUrl}/accounts/{_accountId}/access/policies/{_policyId}";
        var request = BuildRequest(HttpMethod.Get, url);

        var response = await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Cloudflare API error: {response.StatusCode} - {content}");
        }

        var result = JsonSerializer.Deserialize<CloudflareResponse<AccessReusablePolicy>>(content);
        return result?.Result;
    }

    public async Task<bool> UpdateReusablePolicyAsync(IPAddress ipAddressV4, IPAddress? ipAddressV6, AccessReusablePolicy policy, int? ipv6PrefixLength = null, string? country = null)
    {
        var url = $"{_cloudflareBaseUrl}/accounts/{_accountId}/access/policies/{_policyId}";

        var updatedPolicy = new UpdateReusablePolicy(policy, ipAddressV4, ipAddressV6, ipv6PrefixLength, country);
        var jsonPayload = JsonSerializer.Serialize(updatedPolicy);

        var request = BuildRequest(HttpMethod.Put, url, jsonPayload);

        var response = await _client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"Error: {content}");
            return false;
        }

        var result = JsonSerializer.Deserialize<CloudflareResponse<object>>(content);
        return result?.Success ?? false;
    }

    private HttpRequestMessage BuildRequest(HttpMethod method, string url, string? jsonContent = null)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);

        if (method == HttpMethod.Get)
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        else if (method == HttpMethod.Put && jsonContent is not null)
        {
            request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        }

        return request;
    }
}
