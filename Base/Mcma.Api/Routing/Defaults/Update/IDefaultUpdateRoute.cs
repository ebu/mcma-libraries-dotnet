using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Update;

public interface IDefaultUpdateRoute<TResource> : IMcmaApiRoute where TResource : McmaResource
{
}