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
}