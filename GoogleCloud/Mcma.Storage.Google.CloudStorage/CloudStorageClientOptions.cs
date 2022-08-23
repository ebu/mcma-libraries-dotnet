using Google.Cloud.Storage.V1;

namespace Mcma.Storage.Google.CloudStorage;

public class CloudStorageClientOptions
{
    public SigningVersion SigningVersion { get; set; } = SigningVersion.Default;
}