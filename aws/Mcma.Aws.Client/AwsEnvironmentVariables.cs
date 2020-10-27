using System;

namespace Mcma.Aws.Client
{
    public static class AwsEnvironmentVariables
    {
        public static string AccessKey => Environment.GetEnvironmentVariable("Aws_ACCESS_KEY_ID");

        public static string SecretKey => Environment.GetEnvironmentVariable("Aws_SECRET_ACCESS_KEY");

        public static string SessionToken => Environment.GetEnvironmentVariable("Aws_SESSION_TOKEN");

        public static string Region => Environment.GetEnvironmentVariable("Aws_REGION") ?? Environment.GetEnvironmentVariable("Aws_DEFAULT_REGION");
    }
}