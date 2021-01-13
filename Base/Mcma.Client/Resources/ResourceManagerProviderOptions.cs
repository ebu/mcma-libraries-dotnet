namespace Mcma.Client
{
    public class ResourceManagerProviderOptions
    {
        public ResourceManagerOptions DefaultOptions { get; set; } =
            new ResourceManagerOptions(McmaResourceManagerEnvironmentVariables.ServicesUrl,
                                       McmaResourceManagerEnvironmentVariables.ServicesAuthType,
                                       McmaResourceManagerEnvironmentVariables.ServicesAuthContext);
    }
}