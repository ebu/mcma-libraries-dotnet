using System;
using Mcma.Model;

namespace Mcma.Logging.Aws.CloudWatch;

public class CloudWatchLogger : Logger
{
    internal CloudWatchLogger(string source, string requestId, McmaTracker tracker, Action<LogEvent> log)
        : base(source, requestId, tracker)
    {
        LogAction = log;
    }

    private Action<LogEvent> LogAction { get; }

    protected override void WriteLogEvent(LogEvent logEvent)
    {
        if (logEvent.Level > 0)
            LogAction?.Invoke(logEvent);
    }
}