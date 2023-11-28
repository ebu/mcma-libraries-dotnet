
using Mcma.Model;

namespace Mcma.Client.Resources;

public interface IResourceManagerProvider
{
    IResourceManager GetDefault(McmaTracker tracker = null);
    
    IResourceManager Get(ResourceManagerOptions options, McmaTracker tracker = null);
}