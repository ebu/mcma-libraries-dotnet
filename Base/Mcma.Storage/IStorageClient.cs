using System;
using System.IO;
using System.Threading.Tasks;

namespace Mcma.Storage
{
    public interface IStorageClient
    {
        Task<string> GetPresignedUrlAsync(string bucket, string objectPath, PresignedUrlAccessType accessType, TimeSpan? validFor = null);
        
        Task DownloadAsync(string bucket, string objectPath, Stream destination, Action<StreamProgress> progressHandler = null);

        Task UploadAsync(string bucket, string objectPath, Stream source, Action<StreamProgress> progressHandler = null);
    }
}