namespace Mcma.Aws.S3
{
    public class AwsS3Locator : UrlLocator
    {
        private S3ParsedUrl ParsedUrl { get; set; } 

        public string Bucket => GetParsedUrl().Bucket;

        internal S3ParsedUrl GetParsedUrl() => ParsedUrl?.Url == Url ? ParsedUrl : ParsedUrl = new S3ParsedUrl(Url);
    }
}