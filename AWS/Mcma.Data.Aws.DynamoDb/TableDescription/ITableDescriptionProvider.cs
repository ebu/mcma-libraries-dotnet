using System.Threading.Tasks;
using Amazon.DynamoDBv2;

namespace Mcma.Data.Aws.DynamoDb.TableDescription
{
    public interface ITableDescriptionProvider
    {
        Task<DynamoDbTableDescription> GetTableDescriptionAsync(IAmazonDynamoDB dynamoDb, string tableName);
    }
}