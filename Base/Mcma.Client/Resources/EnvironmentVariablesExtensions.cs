namespace Mcma.Client
{
    public static class EnvironmentVariablesExtensions
    {
        public static ResourceManagerConfig GetResourceManagerConfig(this IEnvironmentVariables environmentVariables)
            => new ResourceManagerConfig(
                environmentVariables.Get(nameof(ResourceManagerConfig.ServicesUrl)),
                environmentVariables.GetOptional(nameof(ResourceManagerConfig.ServicesAuthType)),
                environmentVariables.GetOptional(nameof(ResourceManagerConfig.ServicesAuthContext)));
    }
} 