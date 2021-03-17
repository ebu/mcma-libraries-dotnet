using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Mcma.Storage;
using Microsoft.Extensions.Options;

namespace Mcma.Azure.BlobStorage
{
    internal class BlobStorageClient : IBlobStorageClient
    {
        public BlobStorageClient(IOptions<BlobStorageClientOptions> options)
        {
            Options = options.Value ?? new BlobStorageClientOptions();
            SharedKeyCredential = new StorageSharedKeyCredential(Options.AccountName, Options.AccountKey);
            ServiceClient = new BlobServiceClient(Options.AccountUri, SharedKeyCredential);
        }
        
        private BlobStorageClientOptions Options { get; }
        
        private StorageSharedKeyCredential SharedKeyCredential { get; }
        
        private BlobServiceClient ServiceClient { get; }

        private string GetUrl(string bucket, string objectPath) => $"{Options.AccountUri.ToString().TrimEnd('/')}/{bucket}/{objectPath.TrimStart('/')}";

        private BlobClient GetBlobClient(string bucket, string objectPath) => ServiceClient.GetBlobContainerClient(bucket).GetBlobClient(objectPath);

        private static BlobSasPermissions TranslateAccessType(PresignedUrlAccessType accessType)
            => accessType switch
            {
                PresignedUrlAccessType.Read => BlobSasPermissions.Read,
                PresignedUrlAccessType.Write => BlobSasPermissions.Write,
                PresignedUrlAccessType.Delete => BlobSasPermissions.Delete,
                _ => throw new ArgumentOutOfRangeException(nameof(accessType),
                                                           accessType,
                                                           $"Value {accessType} is not valid for enum {nameof(PresignedUrlAccessType)}")
            };

        public Task<string> GetPresignedUrlAsync(string bucket, string objectPath, PresignedUrlAccessType accessType, TimeSpan? validFor = null)
        {
            var startsOn = DateTimeOffset.UtcNow;
            var expiresOn = DateTimeOffset.UtcNow.Add(validFor ?? TimeSpan.FromMinutes(15));
            
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = bucket,
                BlobName = objectPath,
                Resource = "b",
                StartsOn = startsOn,
                ExpiresOn = expiresOn
            };
            
            sasBuilder.SetPermissions(TranslateAccessType(accessType));

            return Task.FromResult($"{GetUrl(bucket, objectPath)}?{sasBuilder.ToSasQueryParameters(SharedKeyCredential)}");
        }
        
        public async Task DownloadAsync(string bucket, string objectPath, Stream destination, Action<StreamProgress> progressHandler = null)
        {
            var resp = await GetBlobClient(bucket, objectPath).DownloadAsync();

            await resp.Value.Content.CopyToAsync(destination, resp.Value.Content.CreateProgressHandler(progressHandler));
        }

        public async Task UploadAsync(string bucket, string objectPath, Stream content, Action<StreamProgress> progressHandler = null)
        {
            await GetBlobClient(bucket, objectPath).UploadAsync(content, progressHandler: content.CreateProgressHandler(progressHandler));
        }
    }
}