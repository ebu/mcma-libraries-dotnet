using Amazon.Runtime.CredentialManagement;

namespace Mcma.Client.Aws
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
            new(AwsEnvironmentVariables.AccessKey,
                AwsEnvironmentVariables.SecretKey,
                AwsEnvironmentVariables.Region,
                AwsEnvironmentVariables.SessionToken);

        public static Aws4AuthContext ForProfile(string profileName)
        {
            var sharedCredentialsFile = new SharedCredentialsFile();
            if (!sharedCredentialsFile.TryGetProfile(profileName, out var profile))
                throw new McmaException("AWS profile with name '{profileName}' not found in shared credentials file.");

            var awsCredentials = profile.GetAWSCredentials(sharedCredentialsFile);
            var credentials = awsCredentials.GetCredentials();

            return new Aws4AuthContext(credentials.AccessKey,
                                       credentials.SecretKey,
                                       profile.Region.SystemName,
                                       credentials.UseToken ? credentials.Token : null);
        }
    }
}