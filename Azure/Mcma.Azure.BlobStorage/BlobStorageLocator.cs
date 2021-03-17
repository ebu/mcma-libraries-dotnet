using System;

namespace Mcma.Azure.BlobStorage
{
    public class BlobStorageLocator : UrlLocator
    {
        public BlobStorageLocator()
        {
            ParsedUrl = new Lazy<BlobStorageParsedUrl>(() => BlobStorageParsedUrl.Parse(Url));
        }

        private Lazy<BlobStorageParsedUrl> ParsedUrl { get; }

        /// <summary>
        /// Gets the name of the storage account
        /// </summary>
        /// <value></value>
        public string StorageAccountName => ParsedUrl.Value.StorageAccountName;

        /// <summary>
        /// Gets the share on which the file resides
        /// </summary>
        public string Container => ParsedUrl.Value.Container;
        
        /// <summary>
        /// Gets the path of the file or folder within the container
        /// </summary>
        /// <returns></returns>
        public string Path => ParsedUrl.Value.Path;
    }
}