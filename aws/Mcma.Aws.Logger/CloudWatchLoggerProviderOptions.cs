using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Mcma.Logging;

namespace Mcma.Aws.CloudWatch
{
    public class CloudWatchLoggerProviderOptions : LoggerProviderOptions
    {
        public string LogGroupName { get; set; }
        
        public AWSCredentials Credentials { get; set; }
        
        public AmazonCloudWatchLogsConfig Config { get; set; }
    }
}