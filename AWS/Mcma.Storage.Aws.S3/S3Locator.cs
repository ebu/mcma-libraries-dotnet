using System;
using Mcma.Model;

namespace Mcma.Storage.Aws.S3;

public class S3Locator : Locator
{
    private S3Locator(S3ParsedUrl parsedUrl)
    {
        ParsedUrl = new Lazy<S3ParsedUrl>(() => parsedUrl);
    }

    /// <summary>
    /// Instantiates a new <see cref="S3Locator"/>
    /// </summary>
    public S3Locator()
    {
        ParsedUrl = new Lazy<S3ParsedUrl>(() => S3ParsedUrl.Parse(Url));
    }
        
    private Lazy<S3ParsedUrl> ParsedUrl { get; } 

    /// <summary>
    /// Gets the name of the S3 bucket in which the object resides
    /// </summary>
    public string Bucket => ParsedUrl.Value.Bucket;
        
    /// <summary>
    /// Gets the key of the S3 object
    /// </summary>
    public string Key => ParsedUrl.Value.Key;

    /// <summary>
    /// Gets the region of the bucket in which the S3 object resides
    /// </summary>
    public string Region => ParsedUrl.Value.Region;

    /// <summary>
    /// Checks if the provided url is a valid S3 url by trying to parse it
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static bool IsValidUrl(string url) => S3ParsedUrl.TryParse(url, out _);

    /// <summary>
    /// Tries to create a <see cref="S3Locator" /> from a url
    /// </summary>
    /// <param name="url">The url to try parsing</param>
    /// <param name="locator">The newly-created <see cref="S3Locator"/> if the url was parsed successfully, or null if it was not</param>
    /// <returns>True if the url was parsed and a <see cref="S3Locator"/> was created successfully; otherwise, false</returns>
    public static bool TryCreate(string url, out S3Locator locator)
    {
        var parsed = S3ParsedUrl.TryParse(url, out var parsedUrl);
        locator = parsed ? new(parsedUrl) { Url = url } : default;
        return parsed;
    }
}