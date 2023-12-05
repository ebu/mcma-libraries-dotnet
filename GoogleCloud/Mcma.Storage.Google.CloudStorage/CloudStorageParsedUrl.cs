using System;

namespace Mcma.Storage.Google.CloudStorage;

public class CloudStorageParsedUrl
{
    public const string CloudStorageDomain = "storage.googleapis.com";

    private CloudStorageParsedUrl(string url, string bucket, string name)
    {
        Url = url ?? throw new ArgumentNullException(nameof(url));
        Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
        
    public string Url { get; }
            
    public string Bucket { get; }
        
    public string Name { get; }

    public static bool TryParse(string url, out CloudStorageParsedUrl parsedUrl)
    {
        parsedUrl = null;

        if (string.IsNullOrWhiteSpace(url))
            return false;
            
        var uri = new Uri(url, UriKind.Absolute);

        if (!uri.Host.EndsWith(CloudStorageDomain, StringComparison.OrdinalIgnoreCase))
            return false;

        var hostParts = uri.Host.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries);

        var bucket = hostParts.Length == 4 ? hostParts[0] : uri.Segments[1].TrimEnd('/');
        var key = uri.Segments[hostParts.Length == 4 ? 1 : 2].TrimEnd('/');

        parsedUrl = new CloudStorageParsedUrl(url, bucket, key);
        return true;
    }

    public static CloudStorageParsedUrl Parse(string url)
        =>
        TryParse(url, out var parsedUrl)
            ? parsedUrl
            : throw new McmaException($"'{url}' is not valid Google Cloud Storage url. The url must be an absolute url in the format 'https://{{bucket.?}}{CloudStorageDomain}/{{bucket/?}}{{key?}}'.");
}