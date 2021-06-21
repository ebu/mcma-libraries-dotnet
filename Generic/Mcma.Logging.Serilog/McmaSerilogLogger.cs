using System;
using Mcma.Logging;
using Mcma.Model;
using Serilog.Events;
using LogEvent = Mcma.Logging.LogEvent;

namespace Mcma.Logging.Serilog
{
    public class McmaSerilogLogger : Logger
    {
        public McmaSerilogLogger(global::Serilog.ILogger logger, string source, string requestId, McmaTracker tracker)
            : base(source, requestId, tracker)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private global::Serilog.ILogger Logger { get; }

        private static LogEventLevel ConvertLogEventLevel(int logLevel)
            => logLevel switch
            {
                var x when x >= LogLevel.Debug => LogEventLevel.Debug,
                var x when x >= LogLevel.Info => LogEventLevel.Information,
                var x when x >= LogLevel.Warn => LogEventLevel.Warning,
                var x when x >= LogLevel.Error => LogEventLevel.Error,
                _ => LogEventLevel.Fatal
            };

        protected override void WriteLogEvent(LogEvent logEvent)
        {
            Logger.Write(
                    ConvertLogEventLevel(logEvent.Level),
                    "{Level} {Type} {Source} {RequestId} {Message} {Args}",
                    logEvent.Level,
                    logEvent.Type,
                    logEvent.Source,
                    logEvent.RequestId,
                    logEvent.Message,
                    logEvent.Args);
        }
    }
}