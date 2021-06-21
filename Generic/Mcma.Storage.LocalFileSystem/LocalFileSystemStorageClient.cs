using System;
using System.IO;
using System.Threading.Tasks;

namespace Mcma.Storage.LocalFileSystem
{
    public class LocalFileSystemStorageClient : ILocalFileSystemStorageClient
    {
        public Task<string> GetPresignedUrlAsync(string url, PresignedUrlAccessType accessType, TimeSpan? validFor = null)
            => Task.FromResult(url);

        public async Task DownloadAsync(string url, Stream destination, Action<StreamProgress> progressHandler = null)
        {
            var localFile = new LocalFileSystemLocator {Url = url};
            
            using var readStream = File.OpenRead(localFile.LocalPath);
            await readStream.CopyToAsync(destination, readStream.CreateProgressHandler(progressHandler));
        }

        public async Task UploadAsync(string url, Stream source, Action<StreamProgress> progressHandler = null)
        {
            var localFile = new LocalFileSystemLocator {Url = url};

            var directory = Path.GetDirectoryName(localFile.LocalPath);
            if (directory != null)
                Directory.CreateDirectory(directory);
            
            using var writeStream = File.OpenWrite(localFile.LocalPath);
            await source.CopyToAsync(writeStream, source.CreateProgressHandler(progressHandler));
        }
    }
}