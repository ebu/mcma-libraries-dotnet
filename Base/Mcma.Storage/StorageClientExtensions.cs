using System;
using System.IO;
using System.Threading.Tasks;

namespace Mcma.Storage
{
    public static class StorageClientExtensions
    {
        public static async Task DownloadToFileAsync(this IStorageClient storageClient,
                                                     string bucket,
                                                     string objectPath,
                                                     string downloadTo,
                                                     Action<StreamProgress> progressHandler = null)
        {
            using var destinationStream = File.OpenWrite(downloadTo);
            await storageClient.DownloadAsync(bucket, objectPath, destinationStream, progressHandler);
        }

        public static async Task UploadFromFileAsync(this IStorageClient storageClient,
                                                     string bucket,
                                                     string objectPath,
                                                     string uploadFrom,
                                                     Action<StreamProgress> progressHandler = null)
        {
            using var sourceStream = File.OpenRead(uploadFrom);
            await storageClient.UploadAsync(bucket, objectPath, sourceStream, progressHandler);
        }
    }
}