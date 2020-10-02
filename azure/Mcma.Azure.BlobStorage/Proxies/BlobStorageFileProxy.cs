using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;

namespace Mcma.Azure.BlobStorage.Proxies
{
    public class BlobStorageFileProxy : BlobStorageProxy<BlobStorageFileLocator>
    {
        internal BlobStorageFileProxy(BlobStorageFileLocator locator, string connectionString)
            : base(locator, connectionString)
        {
            BlobClient = ContainerClient.GetBlockBlobClient(locator.FilePath);
        }

        public BlockBlobClient BlobClient { get; }

        public async Task<Stream> GetAsync(Stream writeTo = null)
        {
            writeTo = writeTo ?? new MemoryStream();

            await BlobClient.DownloadToAsync(writeTo);

            return writeTo;
        }

        public async Task<string> GetAsTextAsync()
        {
            var memoryStream = new MemoryStream();
            await GetAsync(memoryStream);
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        public string GetPublicReadOnlyUrl(TimeSpan? validFor = null)
        {
            var startsOn = DateTimeOffset.UtcNow;
            var expiresOn = DateTimeOffset.UtcNow.Add(validFor ?? TimeSpan.FromMinutes(15));
            
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = Locator.Container,
                BlobName = Locator.FilePath,
                Resource = "b",
                StartsOn = startsOn,
                ExpiresOn = expiresOn
            };
            
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return BlobClient.Uri + "?" + sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(ServiceClient.AccountName, AccountKey)).ToString();
        }
    }
}
