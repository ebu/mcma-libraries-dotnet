using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Amazon.Runtime.Credentials;

namespace Mcma.Logging.Aws.CloudWatch;

public class CloudWatchLoggerProviderOptions : LoggerProviderOptions
{
    public string LogGroupName { get; set; } = McmaCloudWatchEnvironmentVariables.LogGroupName;

    public AWSCredentials Credentials { get; set; } = DefaultAWSCredentialsIdentityResolver.GetCredentials();
        
    public AmazonCloudWatchLogsConfig Config { get; set; } = new();
}