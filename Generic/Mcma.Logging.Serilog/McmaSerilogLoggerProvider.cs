using Mcma.Model;
using Microsoft.Extensions.Options;

namespace Mcma.Logging.Serilog;

public class McmaSerilogLoggerProvider : LoggerProvider<McmaSerilogLogger>
{
    public McmaSerilogLoggerProvider(global::Serilog.ILogger logger, IOptions<LoggerProviderOptions> options)
        : base(options)
    {
        Logger = logger ?? global::Serilog.Log.Logger;
    }

    private global::Serilog.ILogger Logger { get; }

    protected override McmaSerilogLogger Get(string source, string requestId, McmaTracker tracker) => new(Logger, source, requestId, tracker);
}