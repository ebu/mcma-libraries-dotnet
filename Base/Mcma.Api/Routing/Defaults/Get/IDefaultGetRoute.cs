using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Get
{
    public interface IDefaultGetRoute<TResource> : IMcmaApiRoute where TResource : McmaResource
    {
    }
}