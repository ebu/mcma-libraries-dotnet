using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Mcma.Data;

namespace Mcma.Aws.DynamoDb
{
    public class DynamoDbTableProvider : IDocumentDatabaseTableProvider
    {
        public DynamoDbTableProvider(DynamoDbTableProviderOptions providerOptions = null, IAmazonDynamoDB dynamoDb = null)
        {
            Options = providerOptions ?? new DynamoDbTableProviderOptions();
            DynamoDb = dynamoDb ?? new AmazonDynamoDBClient();
        }
        
        private DynamoDbTableProviderOptions Options { get; }
        
        private IAmazonDynamoDB DynamoDb { get; }

        public async Task<IDocumentDatabaseTable> GetAsync(string tableName)
            => new DynamoDbTable(DynamoDb, await DynamoDb.GetTableDescriptionAsync(tableName), Options);
    }
}