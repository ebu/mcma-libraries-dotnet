using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Query;

public interface IDefaultQueryRoute<TResource> : IMcmaApiRoute where TResource : McmaResource
{
}