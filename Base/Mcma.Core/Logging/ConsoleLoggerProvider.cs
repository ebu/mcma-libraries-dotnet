using Mcma.Model;
using Microsoft.Extensions.Options;

namespace Mcma.Logging
{
    /// <summary>
    /// Represents a logger provider that creates loggers that write to the <see cref="Console"/>
    /// </summary>
    public class ConsoleLoggerProvider : LoggerProvider<ConsoleLogger>
    {
        public ConsoleLoggerProvider(IOptions<LoggerProviderOptions> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets a <see cref="ConsoleLogger"/>
        /// </summary>
        /// <param name="source">The source of the log events to be written</param>
        /// <param name="requestId">The ID of the current request, if any</param>
        /// <param name="tracker">The tracker for the current MCMA operation, if any</param>
        /// <returns>A <see cref="ConsoleLogger"/></returns>
        protected override ConsoleLogger Get(string source, string requestId, McmaTracker tracker)
            => new(Source, requestId, tracker);
    }
}