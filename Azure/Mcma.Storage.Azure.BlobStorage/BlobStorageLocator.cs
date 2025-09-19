using System;
using Mcma.Model;

namespace Mcma.Storage.Azure.BlobStorage;

public class BlobStorageLocator : Locator
{
    private BlobStorageLocator(BlobStorageParsedUrl parsedUrl)
    {
        ParsedUrl = new Lazy<BlobStorageParsedUrl>(() => parsedUrl);
    }

    /// <summary>
    /// Instantiates a new <see cref="BlobStorageLocator"/>
    /// </summary>
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

    /// <summary>
    /// Tries to create a <see cref="BlobStorageLocator" /> from a url
    /// </summary>
    /// <param name="url">The url to try parsing</param>
    /// <param name="locator">The newly-created <see cref="BlobStorageLocator"/> if the url was parsed successfully, or null if it was not</param>
    /// <returns>True if the url was parsed and a <see cref="BlobStorageLocator"/> was created successfully; otherwise, false</returns>
    public static bool TryCreate(string url, out BlobStorageLocator locator)
    {
        var parsed = BlobStorageParsedUrl.TryParse(url, out var parsedUrl);
        locator = parsed ? new(parsedUrl) { Url = url } : default;
        return parsed;
    }
}