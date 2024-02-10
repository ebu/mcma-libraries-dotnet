
namespace Mcma.Client.Auth;

public class AuthenticatorKey
{
    public const string Wildcard = "*";

    public AuthenticatorKey(string authType, string? serviceName = null, string? resourceType = null)
    {
        AuthType = authType ?? throw new ArgumentNullException(nameof(authType));
        ServiceName = !string.IsNullOrWhiteSpace(serviceName) ? serviceName : Wildcard;
        ResourceType = !string.IsNullOrWhiteSpace(resourceType) ? resourceType : Wildcard;
        Key = $"{AuthType}/{ServiceName}/{ResourceType}";
    }

    public string AuthType { get; }

    public string? ServiceName { get; private set; }

    public string? ResourceType { get; private set; }

    public string Key { get; }

    public static implicit operator string(AuthenticatorKey key) => key.Key;

    public static bool operator ==(AuthenticatorKey key1, string key2) => string.Equals(key1, key2, StringComparison.OrdinalIgnoreCase);

    public static bool operator !=(AuthenticatorKey key1, string key2) => !(key1 == key2);

    public static bool operator ==(string key1, AuthenticatorKey key2) => string.Equals(key1, key2, StringComparison.OrdinalIgnoreCase);

    public static bool operator !=(string key1, AuthenticatorKey key2) => !(key1 == key2);

    public static bool operator ==(AuthenticatorKey key1, AuthenticatorKey key2) => string.Equals(key1, key2, StringComparison.OrdinalIgnoreCase);

    public static bool operator !=(AuthenticatorKey key1, AuthenticatorKey key2) => !(key1 == key2);

    public override string ToString() => Key;

    public override bool Equals(object obj) => (obj is AuthenticatorKey key && key == this) || (obj is string keyString && keyString == this);

    public override int GetHashCode() => Key.GetHashCode();

    public static TKey Create<TKey>(string? serviceName = null, string? resourceType = null) where TKey : AuthenticatorKey, new()
        => new() { ServiceName = serviceName ?? Wildcard, ResourceType = resourceType ?? Wildcard };
}