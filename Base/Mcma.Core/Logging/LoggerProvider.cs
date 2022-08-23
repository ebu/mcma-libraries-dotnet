using System.Threading.Tasks;
using Mcma.Model;
using Microsoft.Extensions.Options;

namespace Mcma.Logging;

/// <summary>
/// Base class for creating <see cref="ILogger"/>s
/// </summary>
/// <typeparam name="T">The type of logger this provides</typeparam>
public abstract class LoggerProvider<T> : ILoggerProvider where T : ILogger
{
    /// <summary>
    /// Instantiates a <see cref="LoggerProvider{T}"/> with options that specify the logger's source
    /// </summary>
    /// <param name="options">Options that specify the logger's source</param>
    protected LoggerProvider(IOptions<LoggerProviderOptions> options)
    {
        Source = options.Value?.Source ?? throw new McmaException("Source not configured for logger provider.");
    }

    /// <summary>
    /// Gets the source for logs that this generates
    /// </summary>
    protected string Source { get; }

    /// <inheritdoc />
    public ILogger Get(string requestId = null, McmaTracker tracker = null) => Get(Source, requestId, tracker);

    /// <summary>
    /// Creates a <see cref="T"/> logger with the provided source, request ID, and tracker
    /// </summary>
    /// <param name="source">The source to put on the generated log event</param>
    /// <param name="requestId">The ID of the current request, if any. This is generally a unique ID provided by the platform on which the code is running.</param>
    /// <param name="tracker">The tracker for the current MCMA operation, if any</param>
    /// <returns>A logger of type <see cref="T"/></returns>
    protected abstract T Get(string source, string requestId, McmaTracker tracker);

    /// <summary>
    /// Allow derived classes to specify how to flush the logs, if supported; if not overridden, this just returns <see cref="Task.CompletedTask"/>
    /// </summary>
    /// <returns></returns>
    public virtual Task FlushAsync() => Task.CompletedTask;
}