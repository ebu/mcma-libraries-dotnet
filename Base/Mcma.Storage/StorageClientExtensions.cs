using System;
using System.IO;
using System.Threading.Tasks;

namespace Mcma.Storage
{
    public static class StorageClientExtensions
    {
        public static async Task DownloadToFileAsync(this IStorageClient storageClient,
                                                     string url,
                                                     string downloadTo,
                                                     Action<StreamProgress> progressHandler = null)
        {
            using var destinationStream = File.OpenWrite(downloadTo);
            await storageClient.DownloadAsync(url, destinationStream, progressHandler);
        }

        public static async Task UploadFromFileAsync(this IStorageClient storageClient,
                                                     string url,
                                                     string uploadFrom,
                                                     Action<StreamProgress> progressHandler = null)
        {
            using var sourceStream = File.OpenRead(uploadFrom);
            await storageClient.UploadAsync(url, sourceStream, progressHandler);
        }
    }
}