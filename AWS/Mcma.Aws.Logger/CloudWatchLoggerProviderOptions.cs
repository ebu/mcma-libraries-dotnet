﻿using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Mcma.Logging;

namespace Mcma.Aws.CloudWatch
{
    public class CloudWatchLoggerProviderOptions : LoggerProviderOptions
    {
        public string LogGroupName { get; set; } = McmaCloudWatchEnvironmentVariables.LogGroupName;

        public AWSCredentials Credentials { get; set; } = FallbackCredentialsFactory.GetCredentials();
        
        public AmazonCloudWatchLogsConfig Config { get; set; } = new AmazonCloudWatchLogsConfig();
    }
}