namespace Mcma.Storage.Aws.S3
{
    public class MultipartUploadOptions
    {
        public long MinimumSize { get; set; } = 128 * 1024 * 1024;

        public long PartSize { get; set; } = 64 * 1024 * 1024;

        public int MaximumConcurrentRequests { get; set; } = 20;
    }
}