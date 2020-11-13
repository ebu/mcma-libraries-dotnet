namespace Mcma.Azure.BlobStorage.Proxies
{
    public static class BlobStorageProxyExtensions
    {
        public static BlobStorageFileProxy Proxy(this BlobStorageFileLocator locator, string connectionString)
            => new BlobStorageFileProxy(locator, connectionString);

        public static BlobStorageFolderProxy Proxy(this BlobStorageFolderLocator locator, string connectionString)
            => new BlobStorageFolderProxy(locator, connectionString);
    }
}