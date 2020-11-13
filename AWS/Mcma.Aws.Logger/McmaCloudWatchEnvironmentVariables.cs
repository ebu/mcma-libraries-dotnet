using Mcma.Utility;

namespace Mcma.Aws.CloudWatch
{
    public static class McmaCloudWatchEnvironmentVariables
    {
        public static string LogGroupName => McmaEnvironmentVariables.Get("LOG_GROUP_NAME");
    }
}