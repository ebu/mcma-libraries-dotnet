using System;
using Mcma.Model;

namespace Mcma.Storage.Aws.S3;

public class S3Locator : Locator
{
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
}