namespace Mcma.Client
{
    public class ResourceManagerProviderOptions
    {
        public ResourceManagerOptions DefaultOptions { get; set; } =
            !string.IsNullOrWhiteSpace(McmaResourceManagerEnvironmentVariables.ServicesUrl)
                ? new ResourceManagerOptions(McmaResourceManagerEnvironmentVariables.ServicesUrl,
                                             McmaResourceManagerEnvironmentVariables.ServicesAuthType,
                                             McmaResourceManagerEnvironmentVariables.ServicesAuthContext)
                : null;
    }
}