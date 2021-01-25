namespace Mcma.Azure.BlobStorage
{
    public abstract class BlobStorageLocator : UrlLocator
    {
        private BlobStorageParsedUrl ParsedUrl { get; set; }

        /// <summary>
        /// Gets the name of the storage account
        /// </summary>
        /// <value></value>
        public string StorageAccountName => GetParsedUrl().StorageAccountName;

        /// <summary>
        /// Gets the share on which the file resides
        /// </summary>
        public string Container => GetParsedUrl().Container;

        internal BlobStorageParsedUrl GetParsedUrl() => ParsedUrl?.Url == Url ? ParsedUrl : ParsedUrl = new BlobStorageParsedUrl(Url);
    }
}
