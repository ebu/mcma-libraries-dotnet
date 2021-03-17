using Google.Cloud.Storage.V1;

namespace Mcma.GoogleCloud.Storage.Proxies
{
    public class CloudStorageClientOptions
    {
        public SigningVersion SigningVersion { get; set; } = SigningVersion.Default;
    }
}