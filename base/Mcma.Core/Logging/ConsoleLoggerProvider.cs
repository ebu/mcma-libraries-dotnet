using Microsoft.Extensions.Options;

namespace Mcma.Logging
{
    public class ConsoleLoggerProvider : LoggerProvider<ConsoleLogger>
    {
        public ConsoleLoggerProvider(IOptions<LoggerProviderOptions> options)
            : base(options)
        {
        }

        protected override ConsoleLogger Get(string source, string requestId, McmaTracker tracker)
            => new ConsoleLogger(Source, requestId, tracker);
    }
}