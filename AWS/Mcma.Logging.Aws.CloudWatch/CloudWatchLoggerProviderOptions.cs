using Amazon.CloudWatchLogs;
using Amazon.Runtime;

namespace Mcma.Logging.Aws.CloudWatch;

public class CloudWatchLoggerProviderOptions : LoggerProviderOptions
{
    public string LogGroupName { get; set; } = McmaCloudWatchEnvironmentVariables.LogGroupName;

    public AWSCredentials Credentials { get; set; } = FallbackCredentialsFactory.GetCredentials();
        
    public AmazonCloudWatchLogsConfig Config { get; set; } = new();
}