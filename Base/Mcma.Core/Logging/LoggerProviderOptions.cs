namespace Mcma.Logging;

public class LoggerProviderOptions
{
    /// <summary>
    /// Gets or sets the source to be specified on log events created by loggers the provider creates
    /// </summary>
    public string? Source { get; set; } = LoggerEnvironmentVariables.Source;
}