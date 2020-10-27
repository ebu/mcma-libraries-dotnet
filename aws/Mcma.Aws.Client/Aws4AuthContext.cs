namespace Mcma.Aws.Client
{
    public class Aws4AuthContext
    {
        public Aws4AuthContext()
        {
        }

        public Aws4AuthContext(string accessKey, string secretKey, string region, string sessionToken = null)
        {
            AccessKey = accessKey;
            SecretKey = secretKey;
            Region = region;
            SessionToken = sessionToken;
        }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public string Region { get; set; }

        public string SessionToken { get; set; }

        public static Aws4AuthContext Global { get; } =
            new Aws4AuthContext(
                AwsEnvironmentVariables.AccessKey,
                AwsEnvironmentVariables.SecretKey,
                AwsEnvironmentVariables.Region,
                AwsEnvironmentVariables.SessionToken);
    }
}