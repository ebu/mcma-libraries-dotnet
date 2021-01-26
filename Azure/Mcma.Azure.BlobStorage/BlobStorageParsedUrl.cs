using System;
using System.Linq;

namespace Mcma.Azure.BlobStorage
{
    internal class BlobStorageParsedUrl
    {
        public BlobStorageParsedUrl(string url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Parse();
        }
        
        public string Url { get; }
        
        public string StorageAccountName { get; private set; }

        public string Container { get; private set; }
        
        public string Path { get; private set; }

        private void Parse()
        {
            if (Url == null)
                return;
            
            var uri = new Uri(Url, UriKind.Absolute);

            var blobHostIndex = uri.Host.IndexOf(".blob.core.windows.net", StringComparison.OrdinalIgnoreCase);
            if (blobHostIndex < 0)
                return;

            StorageAccountName = uri.Host.Substring(0, blobHostIndex);
            if (uri.Segments.Length <= 1)
                return;
            
            Container = uri.Segments[1].TrimEnd('/');
            if (uri.Segments.Length > 2)
                Path = string.Join("", uri.Segments.Skip(2));
        }
    }
}