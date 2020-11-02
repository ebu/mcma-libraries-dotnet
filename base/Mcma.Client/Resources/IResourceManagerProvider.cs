
namespace Mcma.Client
{
    public interface IResourceManagerProvider
    {
        IResourceManager Get(McmaTracker tracker = null, ResourceManagerOptions options = null);
    }
} 