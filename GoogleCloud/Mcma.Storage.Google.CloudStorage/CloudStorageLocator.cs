using System;
using Mcma.Model;

namespace Mcma.Storage.Google.CloudStorage
{
    public class CloudStorageLocator : Locator
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