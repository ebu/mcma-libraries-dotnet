using Microsoft.Extensions.Options;

namespace Mcma.Logging
{
    public abstract class LoggerProvider<T> : ILoggerProvider where T : ILogger
    {
        protected LoggerProvider(IOptions<LoggerProviderOptions> options)
        {
            Source = options?.Value?.Source ?? throw new McmaException("Source not configured for logger provider.");
        }

        protected string Source { get; }

        public ILogger Get(string requestId = null, McmaTracker tracker = null) => Get(Source, requestId, tracker);

        protected abstract T Get(string source, string requestId, McmaTracker tracker);
    }
}