using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CloudflareIpPolicyUpdater.Models.Cloudflare;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PolicyDecision
{
    [EnumMember(Value = "allow")]
    [StringValue("allow")]
    Allow,

    [EnumMember(Value = "deny")]
    [StringValue("deny")]
    Deny,

    [EnumMember(Value = "bypass")]
    [StringValue("bypass")]
    Bypass,

    [EnumMember(Value = "non_identity")]
    [StringValue("non_identity")]
    NonIdentity
}

[AttributeUsage(AttributeTargets.Field)]
public sealed class StringValueAttribute : Attribute
{
    public StringValueAttribute(string value)
    {
        Value = value;
    }

    public string Value { get; }
}

public static class EnumExtensions
{
    public static string StringValue<T>(this T value)
        where T : Enum
    {
        var fieldName = value.ToString();
        var field = typeof(T).GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
        return field?.GetCustomAttribute<StringValueAttribute>()?.Value ?? fieldName;
    }
}