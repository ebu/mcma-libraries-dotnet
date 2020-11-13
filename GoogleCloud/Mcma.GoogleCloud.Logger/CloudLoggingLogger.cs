using System;
using Mcma.Logging;

namespace Mcma.GoogleCloud.Logger
{
    public class CloudLoggingLogger : Logging.Logger
    {
        public CloudLoggingLogger(CloudLoggingLoggerProvider loggerProvider, string source, string requestId, McmaTracker tracker)
            : base(source, requestId, tracker)
        {
            LoggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
        }
        
        private CloudLoggingLoggerProvider LoggerProvider { get; }
        
        protected override void WriteLogEvent(LogEvent logEvent)
        {
        }
    }
}