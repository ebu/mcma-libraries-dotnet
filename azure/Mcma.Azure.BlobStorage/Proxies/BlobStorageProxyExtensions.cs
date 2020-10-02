using System;
using System.Linq;
using Azure.Storage.Blobs;
using Mcma.Context;

namespace Mcma.Azure.BlobStorage.Proxies
{
    public static class BlobStorageProxyExtensions
    {
        private const string StorageAccountNameKeySuffix = "StorageAccountName";
        private const string StorageConnectionStringKeySuffix = "StorageConnectionString";

        private static string GetConnectionString(IContextVariableProvider contextVariableProvider, string storageAccountName)
        {
            var accountNameSetting =
                contextVariableProvider.GetAllContextVariables()                        
                                       .Where(kvp => !kvp.Key.StartsWith("APPSETTING_"))
                                       .FirstOrDefault(kvp => kvp.Key.EndsWith(StorageAccountNameKeySuffix, StringComparison.OrdinalIgnoreCase)
                                                              && kvp.Value.Equals(storageAccountName, StringComparison.OrdinalIgnoreCase));
            if (accountNameSetting.Key == null)
                throw new Exception($"Storage account '{storageAccountName}' is not configured.");
            
            return
                contextVariableProvider.GetRequiredContextVariable(
                    accountNameSetting.Key.Substring(0, accountNameSetting.Key.Length - StorageAccountNameKeySuffix.Length) + StorageConnectionStringKeySuffix);
        }

        public static BlobStorageFileProxy Proxy(this BlobStorageFileLocator locator, string connectionString)
            => new BlobStorageFileProxy(locator, connectionString);

        public static BlobStorageFileProxy Proxy(this BlobStorageFileLocator locator, IContextVariableProvider contextVariableProvider)
            => new BlobStorageFileProxy(locator, GetConnectionString(contextVariableProvider, locator.StorageAccountName));

        public static BlobStorageFolderProxy Proxy(this BlobStorageFolderLocator locator, string connectionString)
            => new BlobStorageFolderProxy(locator, connectionString);

        public static BlobStorageFolderProxy Proxy(this BlobStorageFolderLocator locator, IContextVariableProvider contextVariableProvider)
            => new BlobStorageFolderProxy(locator, GetConnectionString(contextVariableProvider, locator.StorageAccountName));
    }
}