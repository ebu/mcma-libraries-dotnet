using System;
using Mcma.Model;

namespace Mcma.Logging.Google.CloudLogging;

public class CloudLoggingLogger : Logger
{
    public CloudLoggingLogger(CloudLoggingLoggerProvider loggerProvider, string source, string requestId, McmaTracker tracker)
        : base(source, requestId, tracker)
    {
        LoggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
    }

    private CloudLoggingLoggerProvider LoggerProvider { get; }

    protected override void WriteLogEvent(LogEvent logEvent) => LoggerProvider.AddLogEvent(logEvent);
}