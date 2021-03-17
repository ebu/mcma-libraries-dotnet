﻿using Amazon.Runtime;

namespace Mcma.Aws.S3
{
    public class S3StorageClientOptions
    {
        public AWSCredentials Credentials { get; set; } = FallbackCredentialsFactory.GetCredentials();

        public MultipartUploadOptions MultipartUpload { get; set; } = new();
    }
}