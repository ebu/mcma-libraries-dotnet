using Amazon.DynamoDBv2;
using Amazon.Runtime;

namespace Mcma.Aws.DynamoDb
{
    public class DynamoDbTableProviderOptions
    {
        public bool? ConsistentGet { get; set; }
        
        public bool? ConsistentQuery { get; set; }
        
        public AWSCredentials Credentials { get; set; }
        
        public AmazonDynamoDBConfig Config { get; set; }
    }
}