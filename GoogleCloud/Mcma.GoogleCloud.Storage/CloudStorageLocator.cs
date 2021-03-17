using System;
using Mcma.GoogleCloud.Storage.Proxies;

namespace Mcma.GoogleCloud.Storage
{
    public class CloudStorageLocator : UrlLocator
    {
        public CloudStorageLocator()
        {
            ParsedUrl = new Lazy<CloudStorageParsedUrl>(() => CloudStorageParsedUrl.Parse(Url));
        }
        
        private Lazy<CloudStorageParsedUrl> ParsedUrl { get; }

        public string Bucket => ParsedUrl.Value.Bucket;
        
        public string Name => ParsedUrl.Value.Name;
    }
}