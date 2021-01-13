﻿using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Mcma.Data;

namespace Mcma.Aws.DynamoDb
{
    public class DynamoDbTableOptions : DocumentDatabaseTableOptions
    {
        public bool? ConsistentGet { get; set; }
        
        public bool? ConsistentQuery { get; set; }

        public AWSCredentials Credentials { get; set; } = FallbackCredentialsFactory.GetCredentials();

        public AmazonDynamoDBConfig Config { get; set; } = new AmazonDynamoDBConfig();
    }
}