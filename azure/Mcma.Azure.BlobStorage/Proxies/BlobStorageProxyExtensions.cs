using System;
using System.Linq;

namespace Mcma.Azure.BlobStorage.Proxies
{
    public static class BlobStorageProxyExtensions
    {
        private const string StorageAccountNameKeySuffix = "StorageAccountName";
        private const string StorageConnectionStringKeySuffix = "StorageConnectionString";

        private static string GetConnectionString(IEnvironmentVariables environmentVariables, string storageAccountName)
        {
            var accountNameSettingKey =
                environmentVariables.Keys
                                    .FirstOrDefault(
                                        key =>
                                            !key.StartsWith("APPSETTING_") &&
                                            key.EndsWith(StorageAccountNameKeySuffix, StringComparison.OrdinalIgnoreCase) &&
                                            environmentVariables.Get(key).Equals(storageAccountName, StringComparison.OrdinalIgnoreCase));
            if (accountNameSettingKey == null)
                throw new Exception($"Storage account '{storageAccountName}' is not configured.");
            
            return
                environmentVariables.Get(
                    accountNameSettingKey.Substring(0, accountNameSettingKey.Length - StorageAccountNameKeySuffix.Length) + StorageConnectionStringKeySuffix);
        }

        public static BlobStorageFileProxy Proxy(this BlobStorageFileLocator locator, string connectionString)
            => new BlobStorageFileProxy(locator, connectionString);

        public static BlobStorageFileProxy Proxy(this BlobStorageFileLocator locator, IEnvironmentVariables environmentVariables)
            => new BlobStorageFileProxy(locator, GetConnectionString(environmentVariables, locator.StorageAccountName));

        public static BlobStorageFolderProxy Proxy(this BlobStorageFolderLocator locator, string connectionString)
            => new BlobStorageFolderProxy(locator, connectionString);

        public static BlobStorageFolderProxy Proxy(this BlobStorageFolderLocator locator, IEnvironmentVariables environmentVariables)
            => new BlobStorageFolderProxy(locator, GetConnectionString(environmentVariables, locator.StorageAccountName));
    }
}