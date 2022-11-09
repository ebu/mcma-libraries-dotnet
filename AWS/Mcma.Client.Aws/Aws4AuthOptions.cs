using Amazon.Runtime.CredentialManagement;

namespace Mcma.Client.Aws;

public class Aws4AuthOptions
{
    public string AccessKey { get; set; } = AwsEnvironmentVariables.AccessKey;

    public string SecretKey { get; set; } = AwsEnvironmentVariables.SecretKey;

    public string Region { get; set; } = AwsEnvironmentVariables.Region;

    public string SessionToken { get; set; } = AwsEnvironmentVariables.SessionToken;

    public static Aws4AuthOptions Default { get; } = new();

    public void ForProfile(string profileName)
    {
        var sharedCredentialsFile = new SharedCredentialsFile();
        if (!sharedCredentialsFile.TryGetProfile(profileName, out var profile))
            throw new McmaException("AWS profile with name '{profileName}' not found in shared credentials file.");

        var awsCredentials = profile.GetAWSCredentials(sharedCredentialsFile);
        var credentials = awsCredentials.GetCredentials();

        AccessKey = credentials.AccessKey;
        SecretKey = credentials.SecretKey;
        Region = profile.Region.SystemName;
        SessionToken = credentials.UseToken ? credentials.Token : null;
    }
}