using Amazon.Runtime.CredentialManagement;
using System.Collections.Generic;

namespace Mcma.Client.Aws;

public class Aws4AuthOptions
{
    public string AccessKey { get; set; }

    public string SecretKey { get; set; }

    public string Region { get; set; }

    public string SessionToken { get; set; }

    public void Validate()
    {
        var missingFields = new List<string>();

        if (!string.IsNullOrEmpty(AccessKey))
            missingFields.Add(nameof(AccessKey));

        if (!string.IsNullOrEmpty(SecretKey))
            missingFields.Add(nameof(SecretKey));

        if (!string.IsNullOrEmpty(Region))
            missingFields.Add(nameof(Region));

        if (missingFields.Count > 0)
            throw new McmaException($"The following properties are not set in the AWS4 auth options: {string.Join(", ", missingFields)}");
    }

    public static void ConfigureFromEnvVars(Aws4AuthOptions options)
    {
        options.AccessKey = AwsEnvironmentVariables.AccessKey;
        options.SecretKey = AwsEnvironmentVariables.SecretKey;
        options.Region = AwsEnvironmentVariables.Region;
        options.SessionToken = AwsEnvironmentVariables.SessionToken;
    }

    public static void ConfigureFromProfile(Aws4AuthOptions options, string profileName)
    {
        var sharedCredentialsFile = new SharedCredentialsFile();
        if (!sharedCredentialsFile.TryGetProfile(profileName, out var profile))
            throw new McmaException("AWS profile with name '{profileName}' not found in shared credentials file.");

        var awsCredentials = profile.GetAWSCredentials(sharedCredentialsFile);
        var credentials = awsCredentials.GetCredentials();

        options.AccessKey = credentials.AccessKey;
        options.SecretKey = credentials.SecretKey;
        options.Region = profile.Region.SystemName;
        options.SessionToken = credentials.UseToken ? credentials.Token : null;
    }

    public static Aws4AuthOptions CreateFromEnvironmentVariables()
    {
        var opts = new Aws4AuthOptions();

        ConfigureFromEnvVars(opts);

        return opts;
    }

    public static Aws4AuthOptions CreateFromProfile(string profileName)
    {
        var opts = new Aws4AuthOptions();

        ConfigureFromProfile(opts, profileName);

        return opts;
    }
}