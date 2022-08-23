namespace Mcma.Client.Resources;

public class ResourceManagerProviderOptions
{
    public ResourceManagerOptions DefaultOptions { get; set; } =
        !string.IsNullOrWhiteSpace(McmaResourceManagerEnvironmentVariables.ServicesUrl)
            ? new ResourceManagerOptions(McmaResourceManagerEnvironmentVariables.ServicesUrl,
                                         McmaResourceManagerEnvironmentVariables.ServicesAuthType,
                                         McmaResourceManagerEnvironmentVariables.ServicesAuthContext)
            : null;
}