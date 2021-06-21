using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Delete
{
    public interface IDefaultDeleteRoute<TResource> : IMcmaApiRoute where TResource : McmaResource
    {
    }
}