using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Query;

// ReSharper disable once UnusedTypeParameter - used to map resource types to routes 
public interface IDefaultQueryRoute<TResource> : IMcmaApiRoute where TResource : McmaResource
{
}