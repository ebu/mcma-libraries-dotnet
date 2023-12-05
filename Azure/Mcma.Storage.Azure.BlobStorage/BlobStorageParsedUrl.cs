using System;
using System.Linq;

namespace Mcma.Storage.Azure.BlobStorage;

internal class BlobStorageParsedUrl
{
    public const string BlobStorageDomain = ".blob.core.windows.net";

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

    public static bool TryParse(string url, out BlobStorageParsedUrl parsedUrl)
    {
        parsedUrl = null;

        if (string.IsNullOrWhiteSpace(url))
            return false;

        var uri = new Uri(url, UriKind.Absolute);

        var blobHostIndex = uri.Host.IndexOf(BlobStorageDomain, StringComparison.OrdinalIgnoreCase);
        if (blobHostIndex < 0)
            return false;

        var storageAccountName = uri.Host.Substring(0, blobHostIndex);
        if (uri.Segments.Length <= 1)
            return false;

        var container = uri.Segments[1].TrimEnd('/');
        var path = default(string);
        if (uri.Segments.Length > 2)
            path = string.Join("", uri.Segments.Skip(2));

        parsedUrl = new BlobStorageParsedUrl(url, storageAccountName, container, path);

        return true;
    }

    public static BlobStorageParsedUrl Parse(string url)
        =>
        TryParse(url, out var parsedUrl)
            ? parsedUrl
            : throw new McmaException($"'{url}' is not valid Azure Blob Storage url. The url must be an absolute url in the format 'https://{{containerName}}{BlobStorageDomain}/{{path?}}'.");
}