using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace Mcma.Storage.Azure.BlobStorage;

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

    private BlobClient GetBlobClient(BlobStorageParsedUrl parsedUrl) =>
        ServiceClient.GetBlobContainerClient(parsedUrl.Container).GetBlobClient(parsedUrl.Path);

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

    public Task<string> GetPresignedUrlAsync(string url, PresignedUrlAccessType accessType, TimeSpan? validFor = null)
    {
        var startsOn = DateTimeOffset.UtcNow;
        var expiresOn = DateTimeOffset.UtcNow.Add(validFor ?? TimeSpan.FromMinutes(15));

        var parsedUrl = BlobStorageParsedUrl.Parse(url);
            
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = parsedUrl.Container,
            BlobName = parsedUrl.Path,
            Resource = "b",
            StartsOn = startsOn,
            ExpiresOn = expiresOn
        };
            
        sasBuilder.SetPermissions(TranslateAccessType(accessType));

        return Task.FromResult($"{url}?{sasBuilder.ToSasQueryParameters(SharedKeyCredential)}");
    }
        
    public async Task DownloadAsync(string url, Stream destination, Action<StreamProgress> progressHandler = null)
    {
        var parsedUrl = BlobStorageParsedUrl.Parse(url);
        var resp = await GetBlobClient(parsedUrl).DownloadAsync();

        await resp.Value.Content.CopyToAsync(destination, resp.Value.Content.CreateProgressHandler(progressHandler));
    }

    public async Task UploadAsync(string url, Stream content, Action<StreamProgress> progressHandler = null)
    {
        var parsedUrl = BlobStorageParsedUrl.Parse(url);
        await GetBlobClient(parsedUrl).UploadAsync(content, progressHandler: content.CreateProgressHandler(progressHandler));
    }
}