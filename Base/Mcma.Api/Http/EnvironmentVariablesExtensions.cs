namespace Mcma.Api
{
    public static class EnvironmentVariablesExtensions
    {
        public static string PublicUrl(this IEnvironmentVariables environmentVariables)
            => environmentVariables.Get(nameof(PublicUrl));
    }
}