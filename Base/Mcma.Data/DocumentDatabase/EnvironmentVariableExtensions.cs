namespace Mcma.Data
{
    public static class EnvironmentVariableExtensions
    {
        public static string TableName(this IEnvironmentVariables environmentVariables)
            => environmentVariables.Get(nameof(TableName));
    }
}