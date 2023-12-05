using System;
using Mcma.Model;

namespace Mcma.Storage.Azure.BlobStorage;

public class BlobStorageLocator : Locator
{
    public BlobStorageLocator()
    {
        ParsedUrl = new Lazy<BlobStorageParsedUrl>(() => BlobStorageParsedUrl.Parse(Url));
    }

    private Lazy<BlobStorageParsedUrl> ParsedUrl { get; }

    /// <summary>
    /// Gets the name of the storage account
    /// </summary>
    /// <value></value>
    public string StorageAccountName => ParsedUrl.Value.StorageAccountName;

    /// <summary>
    /// Gets the share on which the file resides
    /// </summary>
    public string Container => ParsedUrl.Value.Container;
        
    /// <summary>
    /// Gets the path of the file or folder within the container
    /// </summary>
    /// <returns></returns>
    public string Path => ParsedUrl.Value.Path;

    /// <summary>
    /// Checks if the provided url is a valid Blob Storage url by trying to parse it
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static bool IsValidUrl(string url) => BlobStorageParsedUrl.TryParse(url, out _);
}