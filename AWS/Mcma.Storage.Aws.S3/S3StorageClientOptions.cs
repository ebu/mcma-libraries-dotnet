using Amazon.Runtime;
using Amazon.Runtime.Credentials;

namespace Mcma.Storage.Aws.S3;

public class S3StorageClientOptions
{
    public AWSCredentials Credentials { get; set; } = DefaultAWSCredentialsIdentityResolver.GetCredentials();

    public MultipartUploadOptions MultipartUpload { get; set; } = new();
}