using System;
using System.Linq;
using Azure.Storage.Blobs;

namespace Mcma.Azure.BlobStorage.Proxies
{
    public abstract class BlobStorageProxy<T> where T : BlobStorageLocator
    {
        protected BlobStorageProxy(T locator, string connectionString)
        {
            Locator = locator;
            ServiceClient = new BlobServiceClient(connectionString);
            ContainerClient = ServiceClient.GetBlobContainerClient(Locator.Container);
            AccountKey = ParseAccountKey(connectionString);
        }

        public T Locator { get; }
        
        protected string AccountKey { get; }

        protected BlobServiceClient ServiceClient { get; }

        protected BlobContainerClient ContainerClient { get; }
        
        private static string ParseAccountKey(string connectionString)
            =>
                connectionString.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                            .Select(nameValuePair => nameValuePair.Split(new[] {'='}, 2))
                            .Where(nameAndValue => nameAndValue.Length == 2)
                            .FirstOrDefault(nameAndValue => nameAndValue[0] == nameof(AccountKey))?[1];
    }
}
