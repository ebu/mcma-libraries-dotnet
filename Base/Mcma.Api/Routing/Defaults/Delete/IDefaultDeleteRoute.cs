using Mcma.Model;

namespace Mcma.Api.Routing.Defaults.Delete;

// ReSharper disable once UnusedTypeParameter - used to map resource types to routes 
public interface IDefaultDeleteRoute<TResource> : IMcmaApiRoute where TResource : McmaResource;