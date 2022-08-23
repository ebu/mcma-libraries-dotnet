using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Mcma.Model;
using Mcma.Serialization;
using Microsoft.Extensions.Options;

namespace Mcma.Logging.Aws.CloudWatch;

public class CloudWatchLoggerProvider : LoggerProvider<CloudWatchLogger>
{
    public CloudWatchLoggerProvider(IOptions<CloudWatchLoggerProviderOptions> options)
        : base(options)
    {
        LogGroupName = options.Value.LogGroupName ?? throw new McmaException("Log group name not configured for CloudWatch logger provider.");
        LogStreamName = options.Value.Source + "-" + Guid.NewGuid();
        CloudWatchLogsClient = new AmazonCloudWatchLogsClient(options.Value.Credentials, options.Value.Config);
    }

    private IAmazonCloudWatchLogs CloudWatchLogsClient { get; }

    private string LogGroupName { get; }

    private string LogStreamName { get; }

    private List<InputLogEvent> LogEvents { get; } = new();

    private object LogEventsLock { get; } = new();

    private bool LogGroupCreated { get; set; }
        
    private bool LogStreamCreated { get; set; }

    private Task ProcessingTask { get; set; }
        
    private string SequenceToken { get; set; }

    private List<InputLogEvent> GetLogEvents()
    {
        List<InputLogEvent> logEvents;
        lock (LogEventsLock)
        {
            logEvents = LogEvents.ToList();
            LogEvents.Clear();
        }
        return logEvents;
    }

    private async Task ProcessBatchAsync()
    {
        try
        {
            await EnsureLogGroupAndStreamCreatedAsync();

            var logEvents = GetLogEvents();
            while (logEvents.Count > 0)
            {
                var request = new PutLogEventsRequest
                {
                    LogEvents = logEvents,
                    LogGroupName = LogGroupName,
                    LogStreamName = LogStreamName,
                    SequenceToken = SequenceToken
                };

                var data = await CloudWatchLogsClient.PutLogEventsAsync(request);

                SequenceToken = data.NextSequenceToken;

                if (data.RejectedLogEventsInfo != null)
                    Logger.System.Error("AwsCloudWatchLogger: Some log events rejected", data.RejectedLogEventsInfo);
                    
                logEvents = GetLogEvents();
            }
        }
        catch (Exception error)
        {
            Logger.System.Error("AwsCloudWatchLogger: Failed to log to CloudWatchLogs", error);
        }
    }

    private async Task EnsureLogGroupAndStreamCreatedAsync()
    {
        if (LogGroupCreated && LogStreamCreated)
            return;

        if (!LogGroupCreated)
        {
            string nextToken = null;

            do
            {
                var data = await CloudWatchLogsClient.DescribeLogGroupsAsync(new DescribeLogGroupsRequest
                {
                    LogGroupNamePrefix = LogGroupName,
                    NextToken = nextToken
                });
                    
                if (data.LogGroups.Any(logGroup => logGroup.LogGroupName == LogGroupName))
                {
                    LogGroupCreated = true;
                    break;
                }

                nextToken = data.NextToken;
            }
            while (!LogGroupCreated && !string.IsNullOrWhiteSpace(nextToken));

            if (!LogGroupCreated) {
                await CloudWatchLogsClient.CreateLogGroupAsync(new CreateLogGroupRequest { LogGroupName = LogGroupName });
                LogGroupCreated = true;
            }
        }
            
        if (!LogStreamCreated)
        {
            await CloudWatchLogsClient.CreateLogStreamAsync(new CreateLogStreamRequest
            {
                LogGroupName = LogGroupName,
                LogStreamName = LogStreamName
            });

            LogStreamCreated = true;
        }
    }

    private void AddLogEvent(LogEvent logEvent)
    {
        if (logEvent == null)
            return;
            
        var messageJson = logEvent.ToMcmaJson().ToString();
        lock (LogEventsLock)
            LogEvents.Add(new InputLogEvent
            {
                Message = messageJson.Substring(0, Math.Min(messageJson.Length, 262144)),
                Timestamp = logEvent.Timestamp.DateTime
            });

        if (ProcessingTask != null) return;
            
        ProcessingTask = ProcessBatchAsync();
        ProcessingTask.ContinueWith(_ => ProcessingTask = null);
    }

    protected override CloudWatchLogger Get(string source, string requestId, McmaTracker tracker)
        => new(source, requestId, tracker, AddLogEvent);

    public override async Task FlushAsync()
    {
        if (ProcessingTask != null)
            await ProcessingTask;
    }
}