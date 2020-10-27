
namespace Mcma.Client
{
    public interface IResourceManagerProvider
    {
        IResourceManager Get(ResourceManagerOptions options = null, McmaTracker tracker = null);
    }
} 