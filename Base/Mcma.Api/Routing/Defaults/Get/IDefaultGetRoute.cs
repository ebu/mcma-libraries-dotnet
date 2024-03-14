using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Get;

// ReSharper disable once UnusedTypeParameter - used to map resource types to routes 
public interface IDefaultGetRoute<TResource> : IMcmaApiRoute where TResource : McmaResource;