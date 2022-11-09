namespace Mcma.Client.Resources;

public class ResourceManagerProviderOptions
{
    public ResourceManagerOptions DefaultOptions { get; set; } =
        !string.IsNullOrWhiteSpace(McmaResourceManagerEnvironmentVariables.ServiceRegistryUrl)
            ? new ResourceManagerOptions(McmaResourceManagerEnvironmentVariables.ServiceRegistryUrl,
                                         McmaResourceManagerEnvironmentVariables.ServiceRegistryAuthType,
                                         McmaResourceManagerEnvironmentVariables.ServiceRegistryAuthContext)
            : null;
}