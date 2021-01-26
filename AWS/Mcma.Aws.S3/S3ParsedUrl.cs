using System;
using System.Linq;

namespace Mcma.Aws.S3
{
    internal class S3ParsedUrl
    {
        public S3ParsedUrl(string url)
        {
            Url = url ?? throw new ArgumentNullException(nameof(url));
            Parse();
        }
        
        public string Url { get; }
            
        public string Bucket { get; private set; }
        
        public string Key { get; private set; }

        private void Parse()
        {
            if (Url == null)
                return;
            
            var uri = new Uri(Url, UriKind.Absolute);

            var s3HostPartIndex = uri.Host.IndexOf(".s3.amazonaws.com", StringComparison.OrdinalIgnoreCase);
            if (s3HostPartIndex > 0)
            {
                Bucket = uri.Host.Substring(0, s3HostPartIndex);
                if (uri.Segments.Length > 1)
                    Key = string.Join("", uri.Segments.Skip(1));
            }
            else if (uri.Segments.Length > 1)
            {
                Bucket = uri.Segments[1].TrimEnd('/');
                if (uri.Segments.Length > 2)
                    Key = string.Join("", uri.Segments.Skip(2));
            }
        }
    }
}