namespace Mcma.Client.Resources;

public class ResourceManagerProviderOptions
{
    public ResourceManagerOptions DefaultOptions { get; set; } = ResourceManagerOptions.FromEnvironmentVariables();
}