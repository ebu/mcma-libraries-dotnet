using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Api;
using Google.Cloud.Logging.Type;
using Google.Cloud.Logging.V2;
using Google.Protobuf.WellKnownTypes;
using Mcma.GoogleCloud.Metadata;
using Mcma.GoogleCloud.Resources;
using Mcma.Logging;
using Mcma.Serialization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Mcma.GoogleCloud.Logger
{
    public class CloudLoggingLoggerProvider : LoggerProvider<CloudLoggingLogger>
    {
        public CloudLoggingLoggerProvider(IMetadataService metadataService,
                                          IMonitoredResourceProvider resourceProvider,
                                          LoggingServiceV2Client loggingServiceV2Client,
                                          IOptions<LoggerProviderOptions> options)
            : base(options)
        {
            MetadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
            ResourceProvider = resourceProvider ?? throw new ArgumentNullException(nameof(resourceProvider));
            LoggingServiceV2Client = loggingServiceV2Client ?? throw new ArgumentNullException(nameof(loggingServiceV2Client));
            LogNameAndResourceTask = new Lazy<Task<(LogName, MonitoredResource)>>(GetLogNameAndResourceAsync);
        }

        private IMetadataService MetadataService { get; }

        private IMonitoredResourceProvider ResourceProvider { get; }

        private LoggingServiceV2Client LoggingServiceV2Client { get; }

        private Lazy<Task<(LogName, MonitoredResource)>> LogNameAndResourceTask { get; }

        private Task ProcessingTask { get; set; }

        private List<LogEntry> LogEntries { get; set; } = new List<LogEntry>();

        private object LogEventsLock { get; } = new object();

        private async Task<(LogName, MonitoredResource)> GetLogNameAndResourceAsync()
            => (
                   LogName.FromProjectLog(await MetadataService.GetProjectIdAsync(), Source),
                   await ResourceProvider.GetCurrentResourceAsync()
               );

        private List<LogEntry> GetLogEntries()
        {
            List<LogEntry> logEntries;
            lock (LogEventsLock)
            {
                logEntries = LogEntries;
                LogEntries = new List<LogEntry>();
            }
            return logEntries;
        }

        private async Task ProcessBatchAsync()
        {
            var (logName, resource) = await LogNameAndResourceTask.Value;
            try
            {
                var logEntries = GetLogEntries();
                while (logEntries.Count > 0)
                {
                    await LoggingServiceV2Client.WriteLogEntriesAsync(logName, resource, new Dictionary<string, string>(), logEntries);
                    
                    logEntries = GetLogEntries();
                }
            }
            catch (Exception error)
            {
                Mcma.Logging.Logger.System.Error("CloudLoggingLogger: Failed to log to Cloud Logging", error);
            }
        }

        private static LogSeverity ConvertToCloudLogSeverity(int logLevel)
            => logLevel switch
            {
                var x when x >= LogLevel.Debug => LogSeverity.Debug,
                var x when x >= LogLevel.Info => LogSeverity.Info,
                var x when x >= LogLevel.Warn => LogSeverity.Warning,
                var x when x >= LogLevel.Error => LogSeverity.Error,
                var x when x >= LogLevel.Fatal => LogSeverity.Critical,
                _ => LogSeverity.Emergency
            };

        internal void AddLogEvent(LogEvent logEvent)
        {
            lock (LogEventsLock)
                LogEntries.Add(new LogEntry
                {
                    Severity = ConvertToCloudLogSeverity(logEvent.Level),
                    Timestamp = Timestamp.FromDateTimeOffset(logEvent.Timestamp),
                    JsonPayload = ((JObject)logEvent.ToMcmaJson()).ToProtobufStruct()
                });

            if (ProcessingTask != null) return;
            
            ProcessingTask = ProcessBatchAsync();
            ProcessingTask.ContinueWith(t => ProcessingTask = null);
        }

        protected override CloudLoggingLogger Get(string source, string requestId, McmaTracker tracker)
            => new CloudLoggingLogger(this, source, requestId, tracker);

        public override Task FlushAsync() => ProcessingTask ?? Task.CompletedTask;
    }
}