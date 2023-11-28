
namespace Mcma.Client.Auth;

public readonly struct AuthenticatorKey(string AuthType, string ServiceName = null, string ResourceType = null)
{
    public const string Wildcard = "*";

    public string AuthType { get; } = AuthType ?? throw new ArgumentNullException(nameof(AuthType));

    public string ServiceName { get; } = ServiceName ?? Wildcard;

    public string ResourceType { get; } = ResourceType ?? Wildcard;

    public string Key { get; } = $"{AuthType}/{ServiceName ?? Wildcard}/{ResourceType ?? Wildcard}";

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
}