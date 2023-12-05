﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using Amazon;

namespace Mcma.Storage.Aws.S3;

internal class S3ParsedUrl
{
    public const string AwsDomain = ".amazonaws.com";

    private S3ParsedUrl(string url, string bucket, string key, string region)
    {
        Url = url ?? throw new ArgumentNullException(nameof(url));
        Bucket = bucket ?? throw new ArgumentNullException(nameof(bucket));
        Key = key ?? throw new ArgumentNullException(nameof(key));
        Region = region;
    }
        
    public string Url { get; }
            
    public string Bucket { get; }
        
    public string Key { get; }
        
    public string Region { get; }

    public static bool TryParse(string url, out S3ParsedUrl parsedUrl)
    {
        parsedUrl = null;

        if (string.IsNullOrWhiteSpace(url))
            return false;
            
        var uri = new Uri(url, UriKind.Absolute);
        var bucket = default(string);
        var region = default(string);

        var regexResult = Regex.Match(uri.Host, @"(?:(.+)\.)?s3(?:[.-]([A-Za-z0-9-]+))?\.amazonaws\.com");
        if (regexResult.Success)
        {
            region = regexResult.Groups[2].Captures.OfType<Capture>().FirstOrDefault()?.Value ?? RegionEndpoint.USEast1.SystemName;
            bucket = regexResult.Groups[1].Captures.OfType<Capture>().FirstOrDefault()?.Value;
        }

        var keySegmentOffset = 1;
        if (bucket == null)
        {
            if (uri.Segments.Length < 2)
                return false;
                
            bucket = uri.Segments[1].TrimEnd('/');
            keySegmentOffset++;
        }
            
        var key = string.Join("", uri.Segments.Skip(keySegmentOffset));

        parsedUrl = new S3ParsedUrl(url, bucket, key, region);

        return true;
    }

    public static S3ParsedUrl Parse(string url)
        =>
        TryParse(url, out S3ParsedUrl parsedUrl)
            ? parsedUrl
            : throw new McmaException($"'{url}' is not valid AWS S3 url. The url must be an absolute url in the format 'https://{{bucket.?}}{{region}}{AwsDomain}/{{bucket/?}}{{key?}}'.");
}