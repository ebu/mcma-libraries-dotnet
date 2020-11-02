using System.Threading.Tasks;
using Amazon.DynamoDBv2;

namespace Mcma.Aws.DynamoDb
{
    public interface ITableDescriptionProvider
    {
        Task<DynamoDbTableDescription> GetTableDescriptionAsync(IAmazonDynamoDB dynamoDb, string tableName);
    }
}