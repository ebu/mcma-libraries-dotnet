using System;

namespace Mcma.Storage.Google.CloudStorage;

internal class CloudStorageParsedUrl
{
    private CloudStorageParsedUrl(string url, string bucket, string name)
    {
        Url = url ?? throw new ArgumentNullException(nameof(url));
        Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
        
    public string Url { get; }
            
    public string Bucket { get; }
        
    public string Name { get; }

    public static CloudStorageParsedUrl Parse(string url)
    {
        if (url == null)
            return null;
            
        var uri = new Uri(url, UriKind.Absolute);

        if (!uri.Host.EndsWith("storage.googleapis.com", StringComparison.OrdinalIgnoreCase))
            throw new McmaException($"Cloud Storage urls are only supported for parent domain 'storage.googleapis.com'");

        var hostParts = uri.Host.Split(new[] {"."}, StringSplitOptions.RemoveEmptyEntries);

        var bucket = hostParts.Length == 4 ? hostParts[0] : uri.Segments[1].TrimEnd('/');
        var key = uri.Segments[hostParts.Length == 4 ? 1 : 2].TrimEnd('/');

        return new CloudStorageParsedUrl(url, bucket, key);
    }
}