
using Mcma.Model;

namespace Mcma.Client.Resources;

public interface IResourceManagerProvider
{
    IResourceManager Get(McmaTracker tracker = null, ResourceManagerOptions options = null);
}