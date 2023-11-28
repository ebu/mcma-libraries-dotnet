using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;

namespace Mcma.Storage.Aws.S3;

internal class S3StorageClient : IS3StorageClient
{
    public S3StorageClient(IOptions<S3StorageClientOptions> options)
    {
        Options = options.Value ?? new();
    }

    private S3StorageClientOptions Options { get; }

    private static HttpVerb TranslateAccessType(PresignedUrlAccessType accessType)
        => accessType switch
        {
            PresignedUrlAccessType.Read => HttpVerb.GET,
            PresignedUrlAccessType.Write => HttpVerb.PUT,
            PresignedUrlAccessType.Delete => HttpVerb.DELETE,
            _ => throw new ArgumentOutOfRangeException(nameof(accessType),
                                                       accessType,
                                                       $"Value {accessType} is not valid for enum ${nameof(PresignedUrlAccessType)}.")
        };

    public Task<string> GetPresignedUrlAsync(string url, PresignedUrlAccessType accessType, TimeSpan? validFor = null)
    {
        using var client = new AmazonS3Client(Options.Credentials);

        var s3ParsedUrl = S3ParsedUrl.Parse(url);

        return Task.FromResult(
            client.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = s3ParsedUrl.Bucket,
                Key = s3ParsedUrl.Key,
                Verb = TranslateAccessType(accessType),
                Expires = DateTime.UtcNow + (validFor ?? TimeSpan.FromMinutes(15))
            }));
    }

    public async Task DownloadAsync(string url, Stream destination, Action<StreamProgress> progressHandler = null)
    {
        using var client = new AmazonS3Client(Options.Credentials);

        var s3ParsedUrl = S3ParsedUrl.Parse(url);
        var resp = await client.GetObjectAsync(s3ParsedUrl.Bucket, s3ParsedUrl.Key);
            
        using var respStream = resp.ResponseStream;
            
        await respStream.CopyToAsync(destination, respStream.CreateProgressHandler(progressHandler));
    }

    public async Task UploadAsync(string url, Stream content, Action<StreamProgress> progressHandler = null)
    {
        using var client = new AmazonS3Client(Options.Credentials);

        using var transferUtility = new TransferUtility(client,
                                                        new TransferUtilityConfig
                                                        {
                                                            MinSizeBeforePartUpload = Options.MultipartUpload.MinimumSize,
                                                            ConcurrentServiceRequests = Options.MultipartUpload.MaximumConcurrentRequests
                                                        });

        var s3ParsedUrl = S3ParsedUrl.Parse(url);
        var uploadRequest = new TransferUtilityUploadRequest
        {
            BucketName = s3ParsedUrl.Bucket,
            Key = s3ParsedUrl.Key,
            InputStream = content,
            PartSize = Options.MultipartUpload.PartSize
        };

        void HandleProgress(object sender, UploadProgressArgs args) =>
            progressHandler?.Invoke(new StreamProgress(args.TransferredBytes, args.TotalBytes));

        uploadRequest.UploadProgressEvent += HandleProgress;

        await transferUtility.UploadAsync(uploadRequest);

        uploadRequest.UploadProgressEvent -= HandleProgress;
    }
}