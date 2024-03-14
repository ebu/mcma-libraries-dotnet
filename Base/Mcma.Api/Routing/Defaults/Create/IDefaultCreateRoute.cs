using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Create;

// ReSharper disable once UnusedTypeParameter - used to map resource types to routes 
public interface IDefaultCreateRoute<TResource> : IMcmaApiRoute where TResource : McmaResource;