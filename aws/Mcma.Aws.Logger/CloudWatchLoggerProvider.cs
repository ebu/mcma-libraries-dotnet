using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CloudWatchLogs;
using Amazon.CloudWatchLogs.Model;
using Mcma.Logging;
using Mcma.Serialization;
using Microsoft.Extensions.Options;

namespace Mcma.Aws.CloudWatch
{
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

        private List<InputLogEvent> LogEvents { get; } = new List<InputLogEvent>();

        private object LogEventsLock { get; } = new object();

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
            lock (LogEventsLock)
                LogEvents.Add(new InputLogEvent { Message = logEvent.ToMcmaJson().ToString(), Timestamp = logEvent.Timestamp.DateTime });

            if (ProcessingTask != null) return;
            
            ProcessingTask = ProcessBatchAsync();
            ProcessingTask.ContinueWith(t => ProcessingTask = null);
        }

        protected override CloudWatchLogger Get(string source, string requestId, McmaTracker tracker) =>
            new CloudWatchLogger(source, requestId, tracker, AddLogEvent);

        public async Task FlushAsync()
        {
            if (ProcessingTask != null)
                await ProcessingTask;
        }
    }
}