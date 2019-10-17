using System;
using System.Linq;
using Mcma.Core.Context;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace Mcma.Azure.BlobStorage.Proxies
{
    public static class BlobStorageProxyExtensions
    {
        private const string StorageAccountNameKeySuffix = "StorageAccountName";
        private const string StorageConnectionStringKeySuffix = "StorageConnectionString";

        private static CloudBlobClient GetBlobClient(string connectionString)
        {
            if (!CloudStorageAccount.TryParse(connectionString, out var storageAccount))
                throw new Exception("Invalid connection string.");

            return storageAccount.CreateCloudBlobClient();
        }

        private static CloudBlobClient GetBlobClient(IContextVariables contextVariables, string storageAccountName)
        {
            var accountNameSetting =
                contextVariables.GetAll()
                                       .Where(kvp => !kvp.Key.StartsWith("APPSETTING_"))
                                       .FirstOrDefault(kvp => kvp.Key.EndsWith(StorageAccountNameKeySuffix, StringComparison.OrdinalIgnoreCase)
                                                              && kvp.Value.Equals(storageAccountName, StringComparison.OrdinalIgnoreCase));
            if (accountNameSetting.Key == null)
                throw new Exception($"Storage account '{storageAccountName}' is not configured.");
            
            return GetBlobClient(
                contextVariables.GetRequired(
                    accountNameSetting.Key.Substring(0, accountNameSetting.Key.Length - StorageAccountNameKeySuffix.Length) + StorageConnectionStringKeySuffix));
        }

        public static BlobStorageFileProxy Proxy(this BlobStorageFileLocator locator, string connectionString)
            => new BlobStorageFileProxy(locator, GetBlobClient(connectionString));

        public static BlobStorageFileProxy Proxy(this BlobStorageFileLocator locator, IContextVariables contextVariables)
            => new BlobStorageFileProxy(locator, GetBlobClient(contextVariables, locator.StorageAccountName));

        public static BlobStorageFolderProxy Proxy(this BlobStorageFolderLocator locator, string connectionString)
            => new BlobStorageFolderProxy(locator, GetBlobClient(connectionString));

        public static BlobStorageFolderProxy Proxy(this BlobStorageFolderLocator locator, IContextVariables contextVariables)
            => new BlobStorageFolderProxy(locator, GetBlobClient(contextVariables, locator.StorageAccountName));
    }
}