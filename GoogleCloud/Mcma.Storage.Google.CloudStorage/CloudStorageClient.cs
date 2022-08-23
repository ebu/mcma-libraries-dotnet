using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace Mcma.Storage.Google.CloudStorage;

internal class CloudStorageClient : ICloudStorageClient
{
    public CloudStorageClient(StorageClient storageClient, IOptions<CloudStorageClientOptions> options)
    {
        StorageClient = storageClient ?? throw new ArgumentNullException(nameof(storageClient));
        Options = options.Value ?? new CloudStorageClientOptions();
    }
        
    private StorageClient StorageClient { get; }
        
    private CloudStorageClientOptions Options { get; }

    private static HttpMethod TranslateAccessType(PresignedUrlAccessType accessType)
        => accessType switch
        {
            PresignedUrlAccessType.Read => HttpMethod.Get,
            PresignedUrlAccessType.Write => HttpMethod.Put,
            PresignedUrlAccessType.Delete => HttpMethod.Delete,
            _ => throw new ArgumentOutOfRangeException(nameof(accessType),
                                                       accessType,
                                                       $"Value {accessType} is not valid for enum {nameof(PresignedUrlAccessType)}")
        };
        
    public Task<string> GetPresignedUrlAsync(string url, PresignedUrlAccessType accessType, TimeSpan? validFor = null)
    {
        if (!(StorageClient.Service.HttpClientInitializer is ServiceAccountCredential credential))
            throw new McmaException("Unable to sign url because storage client is not authenticated with service account credentials.");

        var parsedUrl = CloudStorageParsedUrl.Parse(url);
        var urlSigner = UrlSigner.FromServiceAccountCredential(credential);

        return urlSigner.SignAsync(parsedUrl.Bucket,
                                   parsedUrl.Name,
                                   validFor ?? TimeSpan.FromMinutes(15),
                                   TranslateAccessType(accessType),
                                   Options.SigningVersion);
    }

    public async Task DownloadAsync(string url, Stream destination, Action<StreamProgress> progressHandler = null)
    {
        var parsedUrl = CloudStorageParsedUrl.Parse(url);
        var storageObject = await StorageClient.GetObjectAsync(parsedUrl.Bucket, parsedUrl.Name);

        var progress =
            new Progress<IDownloadProgress>(
                p =>
                    progressHandler?.Invoke(
                        new StreamProgress(p.BytesDownloaded, storageObject.Size.HasValue ? (long)storageObject.Size.Value : long.MaxValue)));

        await StorageClient.DownloadObjectAsync(storageObject, destination, progress: progress);
    }

    public async Task UploadAsync(string url, Stream source, Action<StreamProgress> progressHandler = null)
    {
        var parsedUrl = CloudStorageParsedUrl.Parse(url);
        var storageObject = await StorageClient.GetObjectAsync(parsedUrl.Bucket, parsedUrl.Name);

        var progress =
            new Progress<IUploadProgress>(
                p =>
                    progressHandler?.Invoke(
                        new StreamProgress(p.BytesSent, storageObject.Size.HasValue ? (long)storageObject.Size.Value : long.MaxValue)));

        await StorageClient.UploadObjectAsync(storageObject, source, progress: progress);
    }
}