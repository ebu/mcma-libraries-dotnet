using System;
using System.IO;
using System.Threading.Tasks;

namespace Mcma.Storage;

public interface IStorageClient
{
    Task<string> GetPresignedUrlAsync(string url, PresignedUrlAccessType accessType, TimeSpan? validFor = null);
        
    Task DownloadAsync(string url, Stream destination, Action<StreamProgress> progressHandler = null);

    Task UploadAsync(string url, Stream source, Action<StreamProgress> progressHandler = null);
}