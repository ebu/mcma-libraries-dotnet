﻿using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using StorageObject = Google.Apis.Storage.v1.Data.Object;

namespace Mcma.GoogleCloud.Storage.Proxies
{
    public static class CloudStorageClientExtensions
    {
        public static async Task<CloudStorageFileLocator> UploadToFolderAsync(this StorageClient storageClient,
                                                                              CloudStorageFolderLocator folderLocator,
                                                                              string fileName,
                                                                              Stream readFrom,
                                                                              UploadObjectOptions options = null,
                                                                              Action<IUploadProgress> progressHandler = null)
        {
            var uploadLocator = folderLocator.GetFileLocator(fileName);
            var objectUploader = storageClient.CreateObjectUploader(uploadLocator.ToStorageObject(), readFrom, options);
            if (progressHandler != null)
                objectUploader.ProgressChanged += progressHandler;
            await objectUploader.UploadAsync();
            return uploadLocator;
        }

        public static async Task<CloudStorageFileLocator> UploadFileToFolderAsync(this StorageClient storageClient,
                                                                              CloudStorageFolderLocator folderLocator,
                                                                              string filePath,
                                                                              string fileName = null,
                                                                              UploadObjectOptions options = null,
                                                                              Action<IUploadProgress> progressHandler = null)
        {
            fileName ??= Path.GetFileName(filePath);
            using var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            return await storageClient.UploadToFolderAsync(folderLocator, fileName, fileStream, options, progressHandler);   
        }

        public static async Task<CloudStorageFileLocator> UploadTextToFolderAsync(this StorageClient storageClient,
                                                                                  CloudStorageFolderLocator folderLocator,
                                                                                  string fileName,
                                                                                  string content,
                                                                                  UploadObjectOptions options = null,
                                                                                  Action<IUploadProgress> progressHandler = null)
        {
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return await storageClient.UploadToFolderAsync(folderLocator, fileName, memoryStream, options, progressHandler);
        }

        public static Task DownloadLocatorAsync(this StorageClient storageClient,
                                                CloudStorageFileLocator fileLocator,
                                                Stream downloadTo,
                                                DownloadObjectOptions options = null,
                                                CancellationToken cancellationToken = default,
                                                IProgress<IDownloadProgress> progressHandler = null)
            => storageClient.DownloadObjectAsync(fileLocator.Bucket, fileLocator.FilePath, downloadTo, options, cancellationToken, progressHandler);

        public static async Task DownloadLocatorToFileAsync(this StorageClient storageClient,
                                                      CloudStorageFileLocator fileLocator,
                                                      string filePath,
                                                      DownloadObjectOptions options = null,
                                                      CancellationToken cancellationToken = default,
                                                      IProgress<IDownloadProgress> progressHandler = null)
        {
            using var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write);
            await storageClient.DownloadLocatorAsync(fileLocator, fileStream, options, cancellationToken, progressHandler);
        }

        public static Task<string> GetLocatorSignedUrlAsync(this StorageClient storageClient,
                                                            CloudStorageFileLocator fileLocator,
                                                            TimeSpan? validFor = null,
                                                            SigningVersion signingVersion = SigningVersion.Default,
                                                            CancellationToken cancellationToken = default)
        {
            if (!(storageClient.Service.HttpClientInitializer is ServiceAccountCredential credential))
                throw new McmaException("Unable to sign url because storage client is not authenticated with service account credentials.");

            var urlSigner = UrlSigner.FromServiceAccountCredential(credential);

            return urlSigner.SignAsync(fileLocator.Bucket, fileLocator.FilePath, validFor ?? TimeSpan.FromMinutes(15), HttpMethod.Get, signingVersion, cancellationToken);
        }
    }
}