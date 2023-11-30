namespace Mcma.Client.Resources;

public class ResourceManagerOptions
{
    public ResourceManagerOptions()
    {
    }
        
    public ResourceManagerOptions(string serviceRegistryUrl, string serviceRegistryAuthType = null, string serviceRegistryAuthContext = null)
    {
        ServiceRegistryUrl = serviceRegistryUrl ?? throw new ArgumentNullException(nameof(serviceRegistryUrl));
        ServiceRegistryAuthType = serviceRegistryAuthType;
        ServiceRegistryAuthContext = serviceRegistryAuthContext;
    }

    public string ServiceRegistryUrl { get; set;  }

    public string ServiceRegistryAuthType { get; set; }

    public string ServiceRegistryAuthContext { get; set; }

    public void Validate()
    {
        if (!Uri.IsWellFormedUriString(ServiceRegistryUrl, UriKind.Absolute))
            throw new McmaException($"Invalid services url: {ServiceRegistryUrl}");
    }

    public static void ConfigureFromEnvVars(ResourceManagerOptions options)
    {
        options.ServiceRegistryUrl = McmaResourceManagerEnvironmentVariables.ServiceRegistryUrl ?? "";
        options.ServiceRegistryAuthType = McmaResourceManagerEnvironmentVariables.ServiceRegistryAuthType;
        options.ServiceRegistryAuthContext = McmaResourceManagerEnvironmentVariables.ServiceRegistryAuthContext;
    }
}