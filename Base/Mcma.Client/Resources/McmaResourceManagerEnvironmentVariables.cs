using Mcma.Utility;

namespace Mcma.Client.Resources;

public static class McmaResourceManagerEnvironmentVariables
{
    private const string ServiceRegistryUrlKeySuffix = "SERVICE_REGISTRY_URL";
    public const string ServiceRegistryUrlKey = McmaEnvironmentVariables.Prefix + ServiceRegistryUrlKeySuffix;
    public static string ServiceRegistryUrl => McmaEnvironmentVariables.Get(ServiceRegistryUrlKeySuffix, false);

    private const string ServiceRegistryAuthTypeKeySuffix = "SERVICE_REGISTRY_AUTH_TYPE";
    public const string ServiceRegistryAuthTypeKey= McmaEnvironmentVariables.Prefix + ServiceRegistryAuthTypeKeySuffix;
    public static string ServiceRegistryAuthType => McmaEnvironmentVariables.Get(ServiceRegistryAuthTypeKeySuffix, false);

    private const string ServiceRegistryAuthContextKeySuffix = "SERVICE_REGISTRY_AUTH_CONTEXT";
    public const string ServiceRegistryAuthContextKey = McmaEnvironmentVariables.Prefix + ServiceRegistryAuthContextKeySuffix;
    public static string ServiceRegistryAuthContext => McmaEnvironmentVariables.Get(ServiceRegistryAuthContextKeySuffix, false);
}