using System;
using System.Linq;

namespace Mcma.Azure.BlobStorage
{
    internal class BlobStorageParsedUrl
    {
        private BlobStorageParsedUrl(string url, string storageAccountName, string container, string path)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            StorageAccountName = storageAccountName ?? throw new ArgumentNullException(nameof(storageAccountName));
            Container = container ?? throw new ArgumentNullException(nameof(container));
            Path = path ?? throw new ArgumentNullException(nameof(path));
        }
        
        public string Url { get; }
        
        public string StorageAccountName { get; }

        public string Container { get; }
        
        public string Path { get; }

        public static BlobStorageParsedUrl Parse(string url)
        {
            if (url == null)
                return null;
            
            var uri = new Uri(url, UriKind.Absolute);

            var blobHostIndex = uri.Host.IndexOf(".blob.core.windows.net", StringComparison.OrdinalIgnoreCase);
            if (blobHostIndex < 0)
                return null;

            var storageAccountName = uri.Host.Substring(0, blobHostIndex);
            if (uri.Segments.Length <= 1)
                return null;
            
            var container = uri.Segments[1].TrimEnd('/');
            var path = default(string);
            if (uri.Segments.Length > 2)
                path = string.Join("", uri.Segments.Skip(2));

            return new BlobStorageParsedUrl(url, storageAccountName, container, path);
        }
    }
}