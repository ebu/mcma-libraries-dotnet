using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Api;
using Google.Cloud.Logging.Type;
using Google.Cloud.Logging.V2;
using Google.Protobuf.WellKnownTypes;
using Mcma.Common.Google;
using Mcma.Common.Google.Metadata;
using Mcma.Common.Google.Resources;
using Mcma.Model;
using Mcma.Serialization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Mcma.Logging.Google.CloudLogging;

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

    private List<LogEntry> LogEntries { get; set; } = new();

    private object LogEventsLock { get; } = new();

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
            Logger.System.Error("CloudLoggingLogger: Failed to log to Cloud Logging", error);
        }
    }

    private static LogSeverity ConvertToCloudLogSeverity(int logLevel)
        => logLevel switch
        {
            >= LogLevel.Debug => LogSeverity.Debug,
            >= LogLevel.Info => LogSeverity.Info,
            >= LogLevel.Warn => LogSeverity.Warning,
            >= LogLevel.Error => LogSeverity.Error,
            >= LogLevel.Fatal => LogSeverity.Critical,
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
        ProcessingTask.ContinueWith(_ => ProcessingTask = null);
    }

    protected override CloudLoggingLogger Get(string source, string requestId, McmaTracker tracker)
        => new(this, source, requestId, tracker);

    public override Task FlushAsync() => ProcessingTask ?? Task.CompletedTask;
}