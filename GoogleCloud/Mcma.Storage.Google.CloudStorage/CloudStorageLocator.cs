using System;
using Mcma.Model;

namespace Mcma.Storage.Google.CloudStorage;

public class CloudStorageLocator : Locator
{
    private CloudStorageLocator(CloudStorageParsedUrl parsedUrl)
    {
        ParsedUrl = new Lazy<CloudStorageParsedUrl>(() => parsedUrl);
    }

    public CloudStorageLocator()
    {
        ParsedUrl = new Lazy<CloudStorageParsedUrl>(() => CloudStorageParsedUrl.Parse(Url));
    }
        
    private Lazy<CloudStorageParsedUrl> ParsedUrl { get; }

    public string Bucket => ParsedUrl.Value.Bucket;
        
    public string Name => ParsedUrl.Value.Name;

    public static bool IsValidUrl(string url) => CloudStorageParsedUrl.TryParse(url, out _);

    public static bool TryCreate(string url, out CloudStorageLocator locator)
    {
        var parsed = CloudStorageParsedUrl.TryParse(url, out var parsedUrl);
        locator = parsed ? new(parsedUrl) : default;
        return parsed;
    }
}