using System;
using Azure.Storage.Blobs.Models;
using Mcma.Utility;
using Microsoft.Extensions.Azure;

namespace Mcma.Storage.Azure.BlobStorage
{
    public class BlobStorageClientOptions
    {
        public string AccountName { get; set; } = McmaEnvironmentVariables.Get("BLOB_STORAGE_ACCOUNT_NAME", false);
        
        public string AccountKey { get; set; } = McmaEnvironmentVariables.Get("BLOB_STORAGE_ACCOUNT_KEY", false);

        public Uri AccountUri => new($"https://{AccountName}.blob.core.windows.net", UriKind.Absolute);
    }
}