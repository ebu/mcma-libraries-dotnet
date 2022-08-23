using Mcma.Utility;

namespace Mcma.Logging;

public static class LoggerEnvironmentVariables
{
    public static readonly string Source = McmaEnvironmentVariables.Get("LOGGER_SOURCE", false);
}