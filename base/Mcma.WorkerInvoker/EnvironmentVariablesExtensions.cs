namespace Mcma.WorkerInvoker
{
    public static class EnvironmentVariablesExtensions
    {
        public static string WorkerFunctionId(this IEnvironmentVariables environmentVariables)
            => environmentVariables.Get(nameof(WorkerFunctionId));
    }
}