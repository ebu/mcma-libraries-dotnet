namespace Mcma.Aws.Client
{
    public static class AwsEnvironmentVariables
    {
        public static string AccessKey => EnvironmentVariables.Instance.Get("AWS_ACCESS_KEY_ID");

        public static string SecretKey => EnvironmentVariables.Instance.Get("AWS_SECRET_ACCESS_KEY");

        public static string SessionToken => EnvironmentVariables.Instance.Get("AWS_SESSION_TOKEN");

        public static string Region => EnvironmentVariables.Instance.GetOptional("AWS_REGION") ?? EnvironmentVariables.Instance.Get("AWS_DEFAULT_REGION");
    }
}