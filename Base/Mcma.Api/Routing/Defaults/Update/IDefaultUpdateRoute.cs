using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Update;

// ReSharper disable once UnusedTypeParameter - used to map resource types to routes 
public interface IDefaultUpdateRoute<TResource> : IMcmaApiRoute where TResource : McmaResource
{
}