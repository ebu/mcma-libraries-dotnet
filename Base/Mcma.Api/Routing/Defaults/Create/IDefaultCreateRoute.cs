using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Create
{
    public interface IDefaultCreateRoute<TResource> : IMcmaApiRoute where TResource : McmaResource
    {
    }
}