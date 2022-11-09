using Mcma.Utility;

namespace Mcma.Client.Resources;

public static class McmaResourceManagerEnvironmentVariables
{
    public static string ServiceRegistryUrl => McmaEnvironmentVariables.Get("SERVICE_REGISTRY_URL", false);
    public static string ServiceRegistryAuthType => McmaEnvironmentVariables.Get("SERVICE_REGISTRY_AUTH_TYPE", false);
    public static string ServiceRegistryAuthContext => McmaEnvironmentVariables.Get("SERVICE_REGISTRY_AUTH_CONTEXT", false);
}