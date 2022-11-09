namespace Mcma.Client.Auth;

public readonly record struct AuthenticatorKey(string AuthType, string ServiceName = "", string ResourceType = "")
{
    public string AuthType { get; } = AuthType ?? throw new ArgumentNullException(nameof(AuthType));

    public string ServiceName { get; } = ServiceName;

    public string ResourceType { get; } = ResourceType;

    public override string ToString() => $"AuthType={AuthType}|ServiceName={ServiceName}|ResourceType={ResourceType}";
}